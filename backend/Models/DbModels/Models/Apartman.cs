namespace ApartManBackend.Models.DbModels.Models
{
    public class Apartman:BaseDbModel
    {
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
