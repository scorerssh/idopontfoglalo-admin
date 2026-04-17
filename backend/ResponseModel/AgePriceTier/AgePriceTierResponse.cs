namespace ApartManBackend.ResponseModel.AgePriceTier
{
    public class AgePriceTierResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public decimal Price { get; set; }
        public int AgeRangeLow { get; set; }
        public int AgeRangeHigh { get; set; }
    }
}
