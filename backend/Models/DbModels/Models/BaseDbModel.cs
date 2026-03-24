using System.ComponentModel.DataAnnotations;

namespace ApartManBackend.Models.DbModels.Models
{
    public class BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
