namespace ApartManBackend.Models.DbModels.Models
{
    public class AgePriceTier:BaseDbModel
    {
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public decimal Price { get; set; }

        public int AgeRangeLow { get; set; }

        public int AgeRangeHigh { get; set; }
    }
}
