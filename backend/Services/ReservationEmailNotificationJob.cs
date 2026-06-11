using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

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

            var guestRecipient = CreateMailAddressOrNull(reservation.Email);
            var apartmanUserRecipients = GetApartmanUserRecipients(apartman);
            if (guestRecipient is null && apartmanUserRecipients.Count == 0)
            {
                return;
            }

            using var smtpClient = CreateSmtpClient(smtpSetting);
            if (guestRecipient is not null)
            {
                using var guestMessage = CreateGuestMessage(smtpSetting, reservation, apartman, guestRecipient);
                await SendMessageAsync(smtpClient, guestMessage, reservationId, guestRecipient);
            }

            foreach (var apartmanUserRecipient in apartmanUserRecipients)
            {
                using var apartmanUserMessage = CreateApartmanUserMessage(
                    smtpSetting,
                    reservation,
                    apartman,
                    apartmanUserRecipient);

                await SendMessageAsync(smtpClient, apartmanUserMessage, reservationId, apartmanUserRecipient);
            }
        }

        private async Task SendMessageAsync(
            SmtpClient smtpClient,
            MailMessage message,
            int reservationId,
            MailAddress recipient)
        {
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

        private static List<MailAddress> GetApartmanUserRecipients(Apartman apartman)
        {
            return apartman.Users
                .Select(x => CreateMailAddressOrNull(x.UserEmail))
                .Where(x => x is not null)
                .Cast<MailAddress>()
                .DistinctBy(x => x.Address, StringComparer.OrdinalIgnoreCase)
                .ToList();
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

        private static MailMessage CreateGuestMessage(
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
                Subject = $"Foglalas visszaigazolas - {apartman.Name}",
                Body = BuildGuestHtmlBody(reservation, apartman),
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(recipient);
            return message;
        }

        private static MailMessage CreateApartmanUserMessage(
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
                Subject = $"Uj foglalas erkezett - {apartman.Name}",
                Body = BuildApartmanUserHtmlBody(reservation, apartman),
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(recipient);
            return message;
        }

        private static string BuildGuestHtmlBody(Reservation reservation, Apartman apartman)
        {
            return BuildHtmlLayout(
                "Foglalas visszaigazolva",
                $"Kedves {reservation.Name}!",
                $"Koszonjuk a foglalast. Az alabbi adatokkal rogzitettuk a foglalasodat a(z) {apartman.Name} apartmanhoz.",
                BuildReservationDetailsHtml(reservation, includeGuestContact: false));
        }

        private static string BuildApartmanUserHtmlBody(Reservation reservation, Apartman apartman)
        {
            return BuildHtmlLayout(
                "Uj foglalas erkezett",
                apartman.Name,
                "Az apartmanhoz uj weboldali foglalas erkezett. A foglalas adatai:",
                BuildReservationDetailsHtml(reservation, includeGuestContact: true));
        }

        private static string BuildHtmlLayout(string title, string headline, string intro, string detailsHtml)
        {
            return $"""
                <!doctype html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <title>{H(title)}</title>
                </head>
                <body style="margin:0;background:#f4f7fb;color:#111827;font-family:Arial,Helvetica,sans-serif;">
                    <div style="padding:24px;">
                        <div style="max-width:640px;margin:0 auto;background:#ffffff;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;">
                            <div style="background:#275bf6;color:#ffffff;padding:20px 24px;">
                                <div style="font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.06em;opacity:.85;">Apartman foglalas</div>
                                <h1 style="margin:8px 0 0;font-size:24px;line-height:1.25;">{H(headline)}</h1>
                            </div>
                            <div style="padding:24px;">
                                <p style="margin:0 0 18px;font-size:15px;line-height:1.6;color:#374151;">{H(intro)}</p>
                                {detailsHtml}
                                <p style="margin:22px 0 0;font-size:12px;line-height:1.5;color:#6b7280;">Ez egy automatikusan kuldott email, kerlek ne valaszolj ra kozvetlenul.</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
                """;
        }

        private static string BuildReservationDetailsHtml(Reservation reservation, bool includeGuestContact)
        {
            var rows = new List<string>
            {
                BuildDetailRow("Szoba", reservation.Room.Name),
                BuildDetailRow("Erkezes", reservation.StartTIme.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                BuildDetailRow("Tavozas", reservation.EndTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                BuildDetailRow("Vendegek szama", reservation.PearsonCount.ToString(CultureInfo.InvariantCulture)),
                BuildDetailRow("Vegosszeg", $"{reservation.TotalPrice.ToString("N0", CultureInfo.GetCultureInfo("hu-HU"))} Ft")
            };

            if (includeGuestContact)
            {
                rows.Add(BuildDetailRow("Foglalas neve", reservation.Name));
                rows.Add(BuildDetailRow("Telefon", reservation.PhoneNumber));
                rows.Add(BuildDetailRow("Email", reservation.Email));
            }

            if (!string.IsNullOrWhiteSpace(reservation.Description))
            {
                rows.Add(BuildDetailRow("Megjegyzes", reservation.Description));
            }

            return $"""
                <table role="presentation" cellspacing="0" cellpadding="0" style="width:100%;border-collapse:collapse;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;">
                    <tbody>
                        {string.Join(Environment.NewLine, rows)}
                    </tbody>
                </table>
                """;
        }

        private static string BuildDetailRow(string label, string? value)
        {
            return $"""
                <tr>
                    <td style="width:40%;padding:12px 14px;border-bottom:1px solid #e5e7eb;background:#f9fafb;font-size:13px;font-weight:700;color:#4b5563;">{H(label)}</td>
                    <td style="padding:12px 14px;border-bottom:1px solid #e5e7eb;font-size:14px;color:#111827;">{H(value)}</td>
                </tr>
                """;
        }

        private static string H(string? value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
