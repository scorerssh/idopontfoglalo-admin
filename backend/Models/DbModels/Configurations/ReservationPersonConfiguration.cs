using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class ReservationPersonConfiguration : IEntityTypeConfiguration<ReservationPerson>
    {
        public void Configure(EntityTypeBuilder<ReservationPerson> builder)
        {
            builder.ToTable("reservation_persons");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Age)
                .IsRequired();

            builder.Property(x => x.PricePerNight)
                .HasPrecision(18, 2);
        }
    }
}
