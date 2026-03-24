namespace ApartManBackend.Models.DbModels.Models
{
    public class Room:BaseDbModel
    {
        public string Name { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public Apartman Apartman { get; set; } = null!;
        public int ApartmanId { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
