using System.Text;
using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class RoomCalendarService
    {
        private readonly ApartmanDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<RoomCalendarService> _logger;

        public RoomCalendarService(
            ApartmanDbContext db,
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment environment,
            ILogger<RoomCalendarService> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _environment = environment;
            _logger = logger;
        }

        public string GetCalendarFilePath(Guid roomGuidId)
        {
            return Path.Combine(GetCalendarDirectoryPath(), $"{roomGuidId}.ics");
        }

        public async Task<RoomCalendarResult?> GetCalendarFileAsync(Guid roomGuidId, CancellationToken ct)
        {
            var outputPath = GetCalendarFilePath(roomGuidId);
            if (File.Exists(outputPath))
            {
                var content = await File.ReadAllBytesAsync(outputPath, ct);
                return new RoomCalendarResult(Path.GetFileName(outputPath), CalendarContentType, content);
            }

            return await GenerateAndPersistCalendarAsync(roomGuidId, ct);
        }

        public async Task<RoomCalendarResult?> GenerateAndPersistCalendarAsync(Guid roomGuidId, CancellationToken ct)
        {
            var calendar = await GenerateCalendarAsync(roomGuidId, ct);
            if (calendar is null)
            {
                return null;
            }

            Directory.CreateDirectory(GetCalendarDirectoryPath());
            await File.WriteAllBytesAsync(GetCalendarFilePath(roomGuidId), calendar.Content, ct);
            return calendar;
        }

        public async Task<RoomCalendarResult?> GenerateCalendarAsync(Guid roomGuidId, CancellationToken ct)
        {
            var room = await _db.Rooms
                .AsNoTracking()
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.GuidId == roomGuidId, ct);

            if (room is null)
            {
                return null;
            }

            var calendar = CreateBaseCalendar(room);
            var events = new List<CalendarEvent>();

            events.AddRange(room.Reservations
                .OrderBy(x => x.StartTIme)
                .ThenBy(x => x.EndTime)
                .Select(reservation => CreateLocalReservationEvent(room, reservation)));

            events.AddRange(await LoadExternalEventsAsync(room.BookingConnectionUrl, "Booking", ct));
            events.AddRange(await LoadExternalEventsAsync(room.SzallasHuConnectionUrl, "Szallas.hu", ct));

            foreach (var calendarEvent in DeduplicateEvents(events))
            {
                calendar.Events.Add(calendarEvent);
            }

            var serializer = new CalendarSerializer();
            var calendarContent = serializer.SerializeToString(calendar) ?? string.Empty;

            return new RoomCalendarResult(
                $"{room.GuidId}.ics",
                CalendarContentType,
                Encoding.UTF8.GetBytes(calendarContent));
        }

        private static Calendar CreateBaseCalendar(Room room)
        {
            var calendar = new Calendar
            {
                ProductId = "-//ApartManBackend//Room Reservations//HU",
                Scale = "GREGORIAN",
                Method = "PUBLISH"
            };

            calendar.AddProperty("X-WR-CALNAME", room.Name);
            calendar.AddProperty("X-WR-CALDESC", $"Osszefesult foglalasok ehhez a szobahoz: {room.Name}");
            return calendar;
        }

        private async Task<List<CalendarEvent>> LoadExternalEventsAsync(string? connectionUrl, string sourceName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(connectionUrl))
            {
                return [];
            }

            try
            {
                var client = _httpClientFactory.CreateClient(nameof(RoomCalendarService));
                using var response = await client.GetAsync(connectionUrl, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to download {SourceName} iCal feed from {ConnectionUrl}. Status code: {StatusCode}", sourceName, connectionUrl, response.StatusCode);
                    return [];
                }

                var calendarContent = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(calendarContent))
                {
                    return [];
                }

                var calendar = Calendar.Load(calendarContent);
                if (calendar is null)
                {
                    return [];
                }

                return calendar.Events
                    .Select(calendarEvent => CloneExternalEvent(calendarEvent, sourceName))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process {SourceName} iCal feed from {ConnectionUrl}", sourceName, connectionUrl);
                return [];
            }
        }

        private static IReadOnlyList<CalendarEvent> DeduplicateEvents(IEnumerable<CalendarEvent> events)
        {
            return events
                .GroupBy(x => new CalendarEventKey(
                    NormalizeDate(x.Start),
                    NormalizeDate(x.End),
                    x.IsAllDay))
                .Select(x => x.First())
                .ToList();
        }

        private static CalendarEvent CreateLocalReservationEvent(Room room, Reservation reservation)
        {
            return new CalendarEvent
            {
                Uid = $"{room.GuidId}-{reservation.Id}@apartmanbackend.local",
                Summary = "Foglalt",
                Description = $"Foglalas a(z) {room.Name} szobahoz.",
                DtStamp = new CalDateTime(DateTime.UtcNow),
                Created = new CalDateTime(DateTime.SpecifyKind(reservation.CreatedAt, DateTimeKind.Utc)),
                Start = new CalDateTime(reservation.StartTIme.Year, reservation.StartTIme.Month, reservation.StartTIme.Day),
                End = new CalDateTime(reservation.EndTime.Year, reservation.EndTime.Month, reservation.EndTime.Day),
                Status = "CONFIRMED"
            };
        }

        private static CalendarEvent CloneExternalEvent(CalendarEvent sourceEvent, string sourceName)
        {
            return new CalendarEvent
            {
                Uid = string.IsNullOrWhiteSpace(sourceEvent.Uid) ? $"{Guid.NewGuid()}@external" : sourceEvent.Uid,
                Summary = string.IsNullOrWhiteSpace(sourceEvent.Summary) ? "Foglalt" : sourceEvent.Summary,
                Description = AppendSourceName(sourceEvent.Description, sourceName),
                DtStamp = sourceEvent.DtStamp ?? new CalDateTime(DateTime.UtcNow),
                Created = sourceEvent.Created,
                Start = sourceEvent.Start,
                End = sourceEvent.End,
                Status = string.IsNullOrWhiteSpace(sourceEvent.Status) ? "CONFIRMED" : sourceEvent.Status,
                Location = sourceEvent.Location
            };
        }

        private static string AppendSourceName(string? description, string sourceName)
        {
            return string.IsNullOrWhiteSpace(description)
                ? $"Kulso foglalas forrasa: {sourceName}"
                : $"{description}{Environment.NewLine}Kulso foglalas forrasa: {sourceName}";
        }

        private string GetCalendarDirectoryPath()
        {
            return Path.Combine(_environment.ContentRootPath, "GeneratedCalendars");
        }

        private static string NormalizeDate(CalDateTime? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.HasTime
                ? value.AsUtc.ToString("yyyyMMddHHmmss")
                : value.Value.ToString("yyyyMMdd");
        }

        private const string CalendarContentType = "text/calendar; charset=utf-8";

        private sealed record CalendarEventKey(string Start, string End, bool IsAllDay);
    }

    public sealed record RoomCalendarResult(string FileName, string ContentType, byte[] Content);
}
