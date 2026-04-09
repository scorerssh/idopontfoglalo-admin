namespace ApartManBackend.ResponseModel.Room
{
    public class RoomsWithMetaData
    {
        public List<RoomResponse>? Rooms { get; set; }

        public int Count { get; set; }

        public int InUseCount { get; set; }
        public int ActiveCount { get; set; }
        public int InActiveCount { get; set; }

    }
}
