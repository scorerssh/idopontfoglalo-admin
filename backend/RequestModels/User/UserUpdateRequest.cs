using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.RequestModels.User
{
    public class UserUpdateRequest
    {
        public int? Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole? Role { get; set; } = UserRole.User;
    }
}
