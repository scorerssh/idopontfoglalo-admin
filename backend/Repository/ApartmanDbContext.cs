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
        public DbSet<ReservationPerson> ReservationPersons => Set<ReservationPerson>();

        public DbSet<RoomPriceTier> RoomPriceTiers => Set<RoomPriceTier>();

        public DbSet<RoomSpecialPricingRule> RoomSpecialPricingRules => Set<RoomSpecialPricingRule>();
        public DbSet<AgePriceTier> AgePriceTiers => Set<AgePriceTier>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApartmanDbContext).Assembly);
        }
    }
}
