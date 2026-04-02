using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("rooms");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.GuidId)
                .IsUnique();

            builder.Property(x => x.GuidId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.BookingConnectionUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.SzallasHuConnectionUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.MaxCapacity)
                .IsRequired();
        }
    }
}
