using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.ResponseModel.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public List<int>? ApartmanIds { get; set; }
    }
}
