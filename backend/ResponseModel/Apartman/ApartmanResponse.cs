using ApartManBackend.ResponseModel.User;

namespace ApartManBackend.ResponseModel.Apartman
{
    public class ApartmanResponse
    {
        public List<UserResponse>? Useres { get; set; }
        public string Name { get; set; } = null!;
        public int Id { get; set; }
    }
}
