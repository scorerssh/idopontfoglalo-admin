using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Models.DbModels.Models
{
    public class User:BaseDbModel
    {
        public string PasswordHash { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public UserRole Role { get; set; }
        public ICollection<Apartman> Apartmans { get; set; } = new List<Apartman>();
        public Guid AuthGuid { get; set; } = Guid.NewGuid();

    }
}
