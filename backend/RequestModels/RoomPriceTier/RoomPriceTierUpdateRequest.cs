namespace ApartManBackend.RequestModels.RoomPriceTier
{
    public class RoomPriceTierUpdateRequest
    {
        public int? RoomPriceTierId { get; set; }
        public decimal? Price { get; set; }
    }
}
