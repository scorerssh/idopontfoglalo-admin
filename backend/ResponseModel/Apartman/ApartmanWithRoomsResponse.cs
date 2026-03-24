using ApartManBackend.ResponseModel.Room;
using ApartManBackend.ResponseModel.User;

namespace ApartManBackend.ResponseModel.Apartman
{
    public class ApartmanWithRoomsResponse
    {
        public string Name { get; set; } = null!;
        public int Id { get; set; }
        public List<UserResponse>? Users { get; set; }
        public List<RoomResponse>? Rooms { get; set; }
    }
}
