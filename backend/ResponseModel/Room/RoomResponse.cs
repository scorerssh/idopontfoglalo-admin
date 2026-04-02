using ApartManBackend.Models.DbModels.Models;

namespace ApartManBackend.ResponseModel.Room
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public string? BindedApartmanName { get; set; }
        public int ApartmanId { get; set; }
        public Guid GuidId { get; set; }
        public string? BookingConnectionUrl { get; set; }
        public string? SzallasHuConnectionUrl { get; set; }
    }
}
