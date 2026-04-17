namespace ApartManBackend.RequestModels.AgePriceTier
{
    public class AgePriceTierCreateRequest
    {
        public int RoomId { get; set; }
        public decimal Price { get; set; }
        public int AgeRangeLow { get; set; }
        public int AgeRangeHigh { get; set; }
    }
}
