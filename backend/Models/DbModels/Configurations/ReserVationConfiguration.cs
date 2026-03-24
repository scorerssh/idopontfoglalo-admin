using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class ReserVationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("reservations");
                builder.HasOne(r => r.Room)
                    .WithMany(r => r.Reservations)
                    .HasForeignKey(r => r.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
