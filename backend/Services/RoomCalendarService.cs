using System.Security.Cryptography;
using System.Text;
using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.EntityFrameworkCore;
using static ApartManBackend.StaticMambers.Enums;

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
            var room = await SyncExternalReservationsAsync(roomGuidId, ct);
            if (room is null)
            {
                return null;
            }

            var calendar = CreateCalendarResult(room);

            Directory.CreateDirectory(GetCalendarDirectoryPath());
            await File.WriteAllBytesAsync(GetCalendarFilePath(roomGuidId), calendar.Content, ct);
            return calendar;
        }

        public async Task<RoomCalendarResult?> GenerateCalendarAsync(Guid roomGuidId, CancellationToken ct)
        {
            var room = await SyncExternalReservationsAsync(roomGuidId, ct);
            return room is null ? null : CreateCalendarResult(room);
        }

        private async Task<Room?> SyncExternalReservationsAsync(Guid roomGuidId, CancellationToken ct)
        {
            var room = await _db.Rooms
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.GuidId == roomGuidId, ct);

            if (room is null)
            {
                return null;
            }

            var loadResults = new[]
            {
                await LoadExternalReservationsAsync(room.BookingConnectionUrl, ReservationSource.BookingCom, "Booking.com", ct),
                await LoadExternalReservationsAsync(room.SzallasHuConnectionUrl, ReservationSource.SzallasHu, "Szallas.hu", ct)
            };

            var hasChanges = false;

            foreach (var loadResult in loadResults)
            {
                if (!loadResult.Succeeded)
                {
                    continue;
                }

                var existingReservations = room.Reservations
                    .Where(x => x.Source == loadResult.Source && !string.IsNullOrWhiteSpace(x.ExternalSourceReservationId))
                    .ToDictionary(x => x.ExternalSourceReservationId!, StringComparer.Ordinal);

                var incomingReservationIds = loadResult.Reservations
                    .Select(x => x.ExternalSourceReservationId)
                    .ToHashSet(StringComparer.Ordinal);

                foreach (var externalReservation in loadResult.Reservations)
                {
                    if (existingReservations.TryGetValue(externalReservation.ExternalSourceReservationId, out var existingReservation))
                    {
                        hasChanges |= UpdateExternalReservation(existingReservation, externalReservation);
                        continue;
                    }

                    room.Reservations.Add(new Reservation
                    {
                        StartTIme = externalReservation.StartDate,
                        EndTime = externalReservation.EndDate,
                        PearsonCount = 0,
                        Name = externalReservation.Name,
                        PhoneNumber = string.Empty,
                        Email = string.Empty,
                        Description = externalReservation.Description,
                        Source = loadResult.Source,
                        ExternalSourceReservationId = externalReservation.ExternalSourceReservationId,
                        RoomId = room.Id
                    });

                    hasChanges = true;
                }

                var reservationsToRemove = room.Reservations
                    .Where(x =>
                        x.Source == loadResult.Source &&
                        !string.IsNullOrWhiteSpace(x.ExternalSourceReservationId) &&
                        !incomingReservationIds.Contains(x.ExternalSourceReservationId))
                    .ToList();

                foreach (var reservationToRemove in reservationsToRemove)
                {
                    room.Reservations.Remove(reservationToRemove);
                    _db.Reservations.Remove(reservationToRemove);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await _db.SaveChangesAsync(ct);
            }

            return room;
        }

        private async Task<ExternalCalendarLoadResult> LoadExternalReservationsAsync(
            string? connectionUrl,
            ReservationSource source,
            string sourceName,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(connectionUrl))
            {
                return new ExternalCalendarLoadResult(source, true, []);
            }

            try
            {
                var client = _httpClientFactory.CreateClient(nameof(RoomCalendarService));
                using var response = await client.GetAsync(connectionUrl, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to download {SourceName} iCal feed from {ConnectionUrl}. Status code: {StatusCode}", sourceName, connectionUrl, response.StatusCode);
                    return new ExternalCalendarLoadResult(source, false, []);
                }

                var calendarContent = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(calendarContent))
                {
                    return new ExternalCalendarLoadResult(source, true, []);
                }

                var calendar = Calendar.Load(calendarContent);
                if (calendar is null)
                {
                    return new ExternalCalendarLoadResult(source, false, []);
                }

                var reservations = calendar.Events
                    .Select(x => MapExternalReservation(x, sourceName))
                    .Where(x => x is not null)
                    .Cast<ExternalReservationImport>()
                    .ToList();

                return new ExternalCalendarLoadResult(source, true, reservations);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process {SourceName} iCal feed from {ConnectionUrl}", sourceName, connectionUrl);
                return new ExternalCalendarLoadResult(source, false, []);
            }
        }

        private RoomCalendarResult CreateCalendarResult(Room room)
        {
            var calendar = CreateBaseCalendar(room);

            foreach (var reservation in room.Reservations
                         .OrderBy(x => x.StartTIme)
                         .ThenBy(x => x.EndTime)
                         .ThenBy(x => x.Source))
            {
                calendar.Events.Add(CreateReservationEvent(room, reservation));
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

        private static bool UpdateExternalReservation(Reservation reservation, ExternalReservationImport externalReservation)
        {
            var hasChanges = false;

            if (reservation.StartTIme != externalReservation.StartDate)
            {
                reservation.StartTIme = externalReservation.StartDate;
                hasChanges = true;
            }

            if (reservation.EndTime != externalReservation.EndDate)
            {
                reservation.EndTime = externalReservation.EndDate;
                hasChanges = true;
            }

            if (reservation.Name != externalReservation.Name)
            {
                reservation.Name = externalReservation.Name;
                hasChanges = true;
            }

            if (reservation.Description != externalReservation.Description)
            {
                reservation.Description = externalReservation.Description;
                hasChanges = true;
            }

            return hasChanges;
        }

        private static CalendarEvent CreateReservationEvent(Room room, Reservation reservation)
        {
            return new CalendarEvent
            {
                Uid = CreateReservationUid(room, reservation),
                Summary = GetReservationSummary(reservation),
                Description = BuildReservationDescription(room, reservation),
                DtStamp = new CalDateTime(DateTime.UtcNow),
                Created = new CalDateTime(DateTime.SpecifyKind(reservation.CreatedAt, DateTimeKind.Utc)),
                Start = new CalDateTime(reservation.StartTIme.Year, reservation.StartTIme.Month, reservation.StartTIme.Day),
                End = new CalDateTime(reservation.EndTime.Year, reservation.EndTime.Month, reservation.EndTime.Day),
                Status = "CONFIRMED"
            };
        }

        private static ExternalReservationImport? MapExternalReservation(CalendarEvent sourceEvent, string sourceName)
        {
            var startDate = TryGetDateOnly(sourceEvent.Start);
            var endDate = TryGetDateOnly(sourceEvent.End);

            if (!startDate.HasValue || !endDate.HasValue || endDate.Value <= startDate.Value)
            {
                return null;
            }

            var summary = string.IsNullOrWhiteSpace(sourceEvent.Summary)
                ? "Kulso foglalas"
                : sourceEvent.Summary.Trim();

            return new ExternalReservationImport(
                BuildExternalReservationId(sourceEvent),
                startDate.Value,
                endDate.Value,
                summary,
                AppendSourceName(sourceEvent.Description, sourceName));
        }

        private static string BuildExternalReservationId(CalendarEvent calendarEvent)
        {
            if (!string.IsNullOrWhiteSpace(calendarEvent.Uid))
            {
                return calendarEvent.Uid.Trim();
            }

            var normalizedValue = string.Join("|",
                NormalizeDate(calendarEvent.Start),
                NormalizeDate(calendarEvent.End),
                calendarEvent.Summary?.Trim() ?? string.Empty,
                calendarEvent.Description?.Trim() ?? string.Empty,
                calendarEvent.Location?.Trim() ?? string.Empty);

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedValue));
            return Convert.ToHexString(hash);
        }

        private static string CreateReservationUid(Room room, Reservation reservation)
        {
            return reservation.Source == ReservationSource.Website
                ? $"{room.GuidId}-{reservation.Id}@apartmanbackend.local"
                : $"{room.GuidId}-{reservation.Source}-{reservation.ExternalSourceReservationId}@apartmanbackend.external";
        }

        private static string GetReservationSummary(Reservation reservation)
        {
            return reservation.Source switch
            {
                ReservationSource.Website => "Foglalt",
                ReservationSource.BookingCom => "Foglalt (Booking.com)",
                ReservationSource.SzallasHu => "Foglalt (Szallas.hu)",
                _ => "Foglalt"
            };
        }

        private static string BuildReservationDescription(Room room, Reservation reservation)
        {
            var sourceName = reservation.Source switch
            {
                ReservationSource.BookingCom => "Booking.com",
                ReservationSource.SzallasHu => "Szallas.hu",
                _ => "Weboldal"
            };

            if (string.IsNullOrWhiteSpace(reservation.Description))
            {
                return $"Foglalas a(z) {room.Name} szobahoz. Forras: {sourceName}";
            }

            return $"{reservation.Description}{Environment.NewLine}Forras: {sourceName}";
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

        private static DateOnly? TryGetDateOnly(CalDateTime? value)
        {
            if (value is null)
            {
                return null;
            }

            var dateTime = value.HasTime
                ? value.AsUtc
                : value.Value;

            return DateOnly.FromDateTime(dateTime);
        }

        private const string CalendarContentType = "text/calendar; charset=utf-8";

        private sealed record ExternalCalendarLoadResult(
            ReservationSource Source,
            bool Succeeded,
            List<ExternalReservationImport> Reservations);

        private sealed record ExternalReservationImport(
            string ExternalSourceReservationId,
            DateOnly StartDate,
            DateOnly EndDate,
            string Name,
            string Description);
    }

    public sealed record RoomCalendarResult(string FileName, string ContentType, byte[] Content);
}
