namespace ApartManBackend.Models.DbModels.Models
{
    public class RoomPriceTier:BaseDbModel
    {

        public int RoomId { get; set; }

        public Room? Room { get; set; }

        public int GuestCount { get; set; }
        public decimal Price { get; set; }
    }
}
