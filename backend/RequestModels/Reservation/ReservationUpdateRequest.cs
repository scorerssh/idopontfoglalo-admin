namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationUpdateRequest
    {
        public int? ReservationId { get; set; }
        public DateOnly? StartTIme { get; set; }
        public DateOnly? EndTime { get; set; }
        public int? PearsonCount { get; set; }
        public int? RoomId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
    }
}
