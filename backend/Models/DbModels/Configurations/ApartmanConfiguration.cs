using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class ApartmanConfiguration : IEntityTypeConfiguration<Apartman>
    {
        public void Configure(EntityTypeBuilder<Apartman> builder)
        {
            builder.ToTable("apartmans");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(a => a.Users)
                .WithMany(u => u.Apartmans)
                .UsingEntity(j => j.ToTable("user_apartmans"));

            builder.HasMany(a => a.Rooms)
                .WithOne(r => r.Apartman)
                .HasForeignKey(r => r.ApartmanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
