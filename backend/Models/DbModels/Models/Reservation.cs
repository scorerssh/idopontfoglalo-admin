using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Models.DbModels.Models
{
    public class Reservation:BaseDbModel
    {
        public DateOnly StartTIme { get; set; }
        public DateOnly EndTime { get; set; }
        public int PearsonCount { get; set; }

        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ReservationSource Source { get; set; }
        public string? ExternalSourceReservationId { get; set; }


        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

    }
}
