using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static ApartManBackend.StaticMambers.Enums;

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

            builder.Property(x => x.Source)
                .HasConversion<int>()
                .HasDefaultValue(ReservationSource.Website);

            builder.Property(x => x.ExternalSourceReservationId)
                .HasMaxLength(256);

            builder.Property(x => x.TotalPrice)
                .HasPrecision(18, 2);

            builder.HasOne(r => r.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Persons)
                .WithOne(p => p.Reservation)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.RoomId, x.Source, x.ExternalSourceReservationId })
                .IsUnique();
        }
    }
}
