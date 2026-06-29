using ApartManBackend.Models.DbModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartManBackend.Models.DbModels.Configurations
{
    public class ApartmanSmtpSettingConfiguration : IEntityTypeConfiguration<ApartmanSmtpSetting>
    {
        public void Configure(EntityTypeBuilder<ApartmanSmtpSetting> builder)
        {
            builder.ToTable("apartman_smtp_settings");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ApartmanId)
                .IsUnique();

            builder.Property(x => x.Host)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.UserName)
                .HasMaxLength(255);

            builder.Property(x => x.Password)
                .HasMaxLength(1024);

            builder.Property(x => x.SenderEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.SenderName)
                .HasMaxLength(100);

            builder.Property(x => x.GuestEmailIntroTemplate)
                .HasMaxLength(1000);

            builder.Property(x => x.GuestSmsTemplate)
                .HasMaxLength(1000);

            builder.Property(x => x.IsEnabled)
                .HasDefaultValue(true);

            builder.HasOne(x => x.Apartman)
                .WithOne(x => x.SmtpSetting)
                .HasForeignKey<ApartmanSmtpSetting>(x => x.ApartmanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
