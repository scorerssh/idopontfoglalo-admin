namespace ApartManBackend.Models.DbModels.Models
{
    public class ApartmanSmtpSetting : BaseDbModel
    {
        public int ApartmanId { get; set; }
        public Apartman Apartman { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string? SenderName { get; set; }
        public string? GuestEmailIntroTemplate { get; set; }
        public string? GuestSmsTemplate { get; set; }
        public bool UseSsl { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
