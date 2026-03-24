namespace ApartManBackend.Models.DbModels.Models
{
    public class Reservation:BaseDbModel
    {
        public DateOnly StartTIme { get; set; }
        public DateOnly EndTime { get; set; }
        public int PearsonCount { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

    }
}
