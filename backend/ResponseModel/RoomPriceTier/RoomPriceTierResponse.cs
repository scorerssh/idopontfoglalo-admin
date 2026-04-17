namespace ApartManBackend.ResponseModel.RoomPriceTier
{
    public class RoomPriceTierResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int GuestCount { get; set; }
        public decimal Price { get; set; }
    }
}
