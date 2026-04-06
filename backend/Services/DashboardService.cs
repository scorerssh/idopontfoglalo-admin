using ApartManBackend.Repository;
using ApartManBackend.ResponseModel.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class DashboardService
    {
        private readonly ApartmanDbContext _db;

        public DashboardService(ApartmanDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardResponse> GetAsync(int userId, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var nextMonthStart = monthStart.AddMonths(1);

            var reservations = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .SelectMany(r => r.Reservations.Select(reservation => new DashboardReservationData
                {
                    CreatedAt = reservation.CreatedAt,
                    StartTime = reservation.StartTIme,
                    EndTime = reservation.EndTime,
                    RoomId = r.Id,
                    RoomName = r.Name,
                    RoomPrice = r.Price
                }))
                .ToListAsync(ct);

            var activeRooms = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .CountAsync(r => r.Reservations.Any(reservation => reservation.StartTIme <= today && reservation.EndTime > today), ct);

            var totalReservations = reservations.Count;
            var reservationsCreatedThisMonth = reservations.Count(x => x.CreatedAt >= monthStart && x.CreatedAt < nextMonthStart);

            var totalRevenue = reservations.Sum(CalculateRevenue);

            var monthlyRevenues = reservations
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(group => new DashboardMonthlyRevenueResponse
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    Revenue = group.Sum(CalculateRevenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var roomRevenues = reservations
                .GroupBy(x => new { x.RoomId, x.RoomName })
                .Select(group => new DashboardRoomRevenueResponse
                {
                    RoomId = group.Key.RoomId,
                    RoomName = group.Key.RoomName,
                    Revenue = group.Sum(CalculateRevenue)
                })
                .OrderByDescending(x => x.Revenue)
                .ThenBy(x => x.RoomName)
                .ToList();

            return new DashboardResponse
            {
                TotalReservations = totalReservations,
                ActiveRooms = activeRooms,
                ReservationsCreatedThisMonth = reservationsCreatedThisMonth,
                TotalRevenue = totalRevenue,
                MonthlyRevenues = monthlyRevenues,
                RoomRevenues = roomRevenues
            };
        }

        private static decimal CalculateRevenue(DashboardReservationData reservation)
        {
            var totalDays = reservation.EndTime.DayNumber - reservation.StartTime.DayNumber;
            var billableDays = totalDays <= 0 ? 1 : totalDays;
            return reservation.RoomPrice * billableDays;
        }

        private sealed class DashboardReservationData
        {
            public DateTime CreatedAt { get; set; }
            public DateOnly StartTime { get; set; }
            public DateOnly EndTime { get; set; }
            public int RoomId { get; set; }
            public string RoomName { get; set; } = null!;
            public decimal RoomPrice { get; set; }
        }
    }
}
