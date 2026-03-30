using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Repository
{
    public class ApartmanDbContext : DbContext
    {
        public ApartmanDbContext(DbContextOptions<ApartmanDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Apartman> Apartmans => Set<Apartman>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApartmanDbContext).Assembly);
        }
    }
}
