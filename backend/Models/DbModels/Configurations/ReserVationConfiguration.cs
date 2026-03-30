using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class ReserVationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime));

            builder.ToTable("reservations");

            builder.Property(x => x.StartTIme)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");

            builder.Property(x => x.EndTime)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");

            builder.HasOne(r => r.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
