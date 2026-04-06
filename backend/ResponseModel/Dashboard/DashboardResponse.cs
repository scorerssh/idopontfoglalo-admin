namespace ApartManBackend.ResponseModel.Dashboard
{
    public class DashboardResponse
    {
        public int TotalReservations { get; set; }
        public int ActiveRooms { get; set; }
        public int ReservationsCreatedThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<DashboardMonthlyRevenueResponse> MonthlyRevenues { get; set; } = new();
        public List<DashboardRoomRevenueResponse> RoomRevenues { get; set; } = new();
    }

    public class DashboardMonthlyRevenueResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardRoomRevenueResponse
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public decimal Revenue { get; set; }
    }
}
