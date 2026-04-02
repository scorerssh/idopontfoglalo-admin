namespace ApartManBackend.RequestModels.Room
{
    public class RoomCreateRequest
    {
        public string Name { get; set; } = null!;
        public int MaxCapacity { get; set; }

        public int MinCapacity { get; set; }
        public int ApartmanId { get; set; }
        public string? BookingConnectionUrl { get; set; }
        public string? SzallasHuConnectionUrl { get; set; }
    }
}
