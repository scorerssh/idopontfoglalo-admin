using ApartManBackend.Models.DbModels.Models;

namespace ApartManBackend.ResponseModel.Room
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string? BindedApartmanName { get; set; }
        public int ApartmanId { get; set; }
    }
}
