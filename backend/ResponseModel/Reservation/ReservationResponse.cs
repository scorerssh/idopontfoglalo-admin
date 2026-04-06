using ApartManBackend.ResponseModel.Room;

namespace ApartManBackend.ResponseModel.Reservation
{
    public class ReservationResponse
    {
        public int Id { get; set; }
        public DateOnly StartTIme { get; set; }
        public DateOnly EndTime { get; set; }
        public int PearsonCount { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Description { get; set; } = null!;

        public RoomResponse? Room { get; set; }
        public ApartManBackend.StaticMambers.Enums.ReservationSource Source { get; set; }
    }
}
