using System.ComponentModel;

namespace ApartManBackend.Models.DbModels.Models
{
    public class Room:BaseDbModel
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public Apartman Apartman { get; set; } = null!;
        public int ApartmanId { get; set; }
        public Guid GuidId { get; set; } =Guid.NewGuid();
        public string? BookingConnectionUrl { get; set; }
        public string? SzallasHuConnectionUrl { get; set; }
        public List<Reservation> Reservations { get; set; } = new();

        [DefaultValue(true)]
        public bool Active { get; set; }
    }
}
