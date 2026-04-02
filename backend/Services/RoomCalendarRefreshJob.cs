using ApartManBackend.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class RoomCalendarRefreshJob
    {
        private readonly ApartmanDbContext _db;
        private readonly RoomCalendarService _roomCalendarService;
        private readonly ILogger<RoomCalendarRefreshJob> _logger;

        public RoomCalendarRefreshJob(
            ApartmanDbContext db,
            RoomCalendarService roomCalendarService,
            ILogger<RoomCalendarRefreshJob> logger)
        {
            _db = db;
            _roomCalendarService = roomCalendarService;
            _logger = logger;
        }

        public async Task RefreshAllCalendarsAsync()
        {
            var roomGuidIds = await _db.Rooms
                .AsNoTracking()
                .Select(x => x.GuidId)
                .ToListAsync();

            foreach (var roomGuidId in roomGuidIds)
            {
                var calendar = await _roomCalendarService.GenerateAndPersistCalendarAsync(roomGuidId, CancellationToken.None);
                if (calendar is null)
                {
                    continue;
                }
            }

            _logger.LogInformation("Room calendar refresh finished. Generated {CalendarCount} calendar file(s).", roomGuidIds.Count);
        }
    }
}
