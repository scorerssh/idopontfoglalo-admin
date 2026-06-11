using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace ApartManBackend.Services
{
    public class ReservationEmailNotificationJob
    {
        private readonly ApartmanDbContext _db;
        private readonly ILogger<ReservationEmailNotificationJob> _logger;

        public ReservationEmailNotificationJob(ApartmanDbContext db, ILogger<ReservationEmailNotificationJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task SendReservationCreatedEmailsAsync(int reservationId)
        {
            var reservation = await _db.Reservations
                .AsNoTracking()
                .Include(x => x.Persons)
                .Include(x => x.Room)
                    .ThenInclude(x => x.Apartman)
                        .ThenInclude(x => x.SmtpSetting)
                .Include(x => x.Room)
                    .ThenInclude(x => x.Apartman)
                        .ThenInclude(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == reservationId);

            if (reservation is null)
            {
                _logger.LogWarning("Reservation email notification skipped. Reservation {ReservationId} was not found.", reservationId);
                return;
            }

            var apartman = reservation.Room.Apartman;
            var smtpSetting = apartman.SmtpSetting;
            if (smtpSetting is null || !smtpSetting.IsEnabled)
            {
                return;
            }

            var recipients = GetRecipients(reservation, apartman);
            if (recipients.Count == 0)
            {
                return;
            }

            using var smtpClient = CreateSmtpClient(smtpSetting);
            foreach (var recipient in recipients)
            {
                using var message = CreateMessage(smtpSetting, reservation, apartman, recipient);
                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send reservation email notification for reservation {ReservationId} to {Recipient}.",
                        reservationId,
                        recipient.Address);
                }
            }
        }

        private static List<MailAddress> GetRecipients(Reservation reservation, Apartman apartman)
        {
            var rawRecipients = new List<string?>();
            rawRecipients.Add(reservation.Email);
            rawRecipients.AddRange(apartman.Users.Select(x => x.UserEmail));

            return rawRecipients
                .Select(CreateMailAddressOrNull)
                .Where(x => x is not null)
                .Cast<MailAddress>()
                .DistinctBy(x => x.Address, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static MailAddress? CreateMailAddressOrNull(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            try
            {
                return new MailAddress(email.Trim());
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static SmtpClient CreateSmtpClient(ApartmanSmtpSetting smtpSetting)
        {
            var smtpClient = new SmtpClient(smtpSetting.Host, smtpSetting.Port)
            {
                EnableSsl = smtpSetting.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(smtpSetting.UserName))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpSetting.UserName, smtpSetting.Password ?? string.Empty);
            }

            return smtpClient;
        }

        private static MailMessage CreateMessage(
            ApartmanSmtpSetting smtpSetting,
            Reservation reservation,
            Apartman apartman,
            MailAddress recipient)
        {
            var senderName = string.IsNullOrWhiteSpace(smtpSetting.SenderName)
                ? apartman.Name
                : smtpSetting.SenderName;

            var message = new MailMessage
            {
                From = new MailAddress(smtpSetting.SenderEmail, senderName),
                Subject = $"Uj foglalas - {apartman.Name}",
                Body = BuildBody(reservation, apartman),
                IsBodyHtml = false
            };

            message.To.Add(recipient);
            return message;
        }

        private static string BuildBody(Reservation reservation, Apartman apartman)
        {
            return string.Join(Environment.NewLine, new[]
            {
                "Uj foglalas erkezett.",
                string.Empty,
                $"Apartman: {apartman.Name}",
                $"Szoba: {reservation.Room.Name}",
                $"Erkezes: {reservation.StartTIme:yyyy-MM-dd}",
                $"Tavozas: {reservation.EndTime:yyyy-MM-dd}",
                $"Vendegek szama: {reservation.PearsonCount}",
                $"Vegosszeg: {reservation.TotalPrice.ToString("N0", CultureInfo.GetCultureInfo("hu-HU"))} Ft",
                string.Empty,
                $"Foglalas neve: {reservation.Name}",
                $"Telefon: {reservation.PhoneNumber}",
                $"Email: {reservation.Email}",
                $"Megjegyzes: {reservation.Description}"
            });
        }
    }
}
