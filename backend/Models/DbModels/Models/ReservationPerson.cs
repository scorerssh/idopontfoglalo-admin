namespace ApartManBackend.Models.DbModels.Models
{
    public class ReservationPerson : BaseDbModel
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
        public int Age { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
