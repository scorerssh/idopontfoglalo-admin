using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ApartManBackend.Services
{
    public class ReservationEmailNotificationJob
    {
        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);

        private readonly ApartmanDbContext _db;
        private readonly ILogger<ReservationEmailNotificationJob> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ReservationEmailNotificationJob(
            ApartmanDbContext db,
            ILogger<ReservationEmailNotificationJob> logger,
            IBackgroundJobClient backgroundJobClient)
        {
            _db = db;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
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

            if (guestRecipient is not null)
            {
                EnqueueRecipientEmail(reservationId, guestRecipient, ReservationEmailRecipientKind.Guest);
            }

            foreach (var apartmanUserRecipient in apartmanUserRecipients)
            {
                EnqueueRecipientEmail(reservationId, apartmanUserRecipient, ReservationEmailRecipientKind.ApartmanUser);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task SendReservationCreatedEmailToRecipientAsync(
            int reservationId,
            string recipientEmail,
            ReservationEmailRecipientKind recipientKind,
            int retryAttempt)
        {
            var normalizedRetryAttempt = Math.Clamp(retryAttempt, 0, MaxRetryAttempts);
            var sendAttempt = normalizedRetryAttempt + 1;
            var maxSendAttempts = MaxRetryAttempts + 1;
            var recipient = CreateMailAddressOrNull(recipientEmail);
            if (recipient is null)
            {
                _logger.LogWarning(
                    "Reservation email notification skipped. Invalid recipient {Recipient} for reservation {ReservationId}.",
                    recipientEmail,
                    reservationId);
                return;
            }

            var reservation = await _db.Reservations
                .AsNoTracking()
                .Include(x => x.Room)
                    .ThenInclude(x => x.Apartman)
                        .ThenInclude(x => x.SmtpSetting)
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

            try
            {
                using var smtpClient = CreateSmtpClient(smtpSetting);
                using var message = CreateMessage(smtpSetting, reservation, apartman, recipient, recipientKind);
                await smtpClient.SendMailAsync(message);
                _logger.LogInformation(
                    "Reservation email notification sent for reservation {ReservationId} to {Recipient} on attempt {Attempt}/{MaxAttempts}.",
                    reservationId,
                    recipient.Address,
                    sendAttempt,
                    maxSendAttempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send reservation email notification attempt {Attempt}/{MaxAttempts} for reservation {ReservationId} to {Recipient}.",
                    sendAttempt,
                    maxSendAttempts,
                    reservationId,
                    recipient.Address);

                if (normalizedRetryAttempt >= MaxRetryAttempts)
                {
                    _logger.LogError(
                        "Reservation email notification permanently failed after {RetryAttempts} retries for reservation {ReservationId} to {Recipient}.",
                        MaxRetryAttempts,
                        reservationId,
                        recipient.Address);
                    throw new InvalidOperationException(
                        $"Reservation email notification failed after {MaxRetryAttempts} retries for reservation {reservationId} to {recipient.Address}.",
                        ex);
                }

                _backgroundJobClient.Schedule<ReservationEmailNotificationJob>(
                    job => job.SendReservationCreatedEmailToRecipientAsync(
                        reservationId,
                        recipient.Address,
                        recipientKind,
                        normalizedRetryAttempt + 1),
                    RetryDelay);
            }
        }

        private void EnqueueRecipientEmail(
            int reservationId,
            MailAddress recipient,
            ReservationEmailRecipientKind recipientKind)
        {
            _backgroundJobClient.Enqueue<ReservationEmailNotificationJob>(
                job => job.SendReservationCreatedEmailToRecipientAsync(
                    reservationId,
                    recipient.Address,
                    recipientKind,
                    0));
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
                EnableSsl = smtpSetting.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
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
            MailAddress recipient,
            ReservationEmailRecipientKind recipientKind)
        {
            return recipientKind switch
            {
                ReservationEmailRecipientKind.Guest => CreateGuestMessage(smtpSetting, reservation, apartman, recipient),
                ReservationEmailRecipientKind.ApartmanUser => CreateApartmanUserMessage(smtpSetting, reservation, apartman, recipient),
                _ => throw new ArgumentOutOfRangeException(nameof(recipientKind), recipientKind, "Unknown email recipient kind.")
            };
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
                Subject = $"Foglalás visszaigazolás - {apartman.Name}",
                Body = BuildGuestHtmlBody(smtpSetting, reservation, apartman),
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
                Subject = $"Új foglalás érkezett - {apartman.Name}",
                Body = BuildApartmanUserHtmlBody(reservation, apartman),
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(recipient);
            return message;
        }

        private static string BuildGuestHtmlBody(ApartmanSmtpSetting smtpSetting, Reservation reservation, Apartman apartman)
        {
            var intro = ReservationNotificationTemplateRenderer.RenderGuestEmailIntro(smtpSetting, reservation, apartman);

            return BuildHtmlLayout(
                "Foglalás visszaigazolva",
                $"Kedves {reservation.Name}!",
                intro,
                BuildReservationDetailsHtml(reservation, includeGuestContact: false));
        }

        private static string BuildApartmanUserHtmlBody(Reservation reservation, Apartman apartman)
        {
            return BuildHtmlLayout(
                "Új foglalás érkezett",
                apartman.Name,
                "Az apartmanhoz új weboldali foglalás érkezett. A foglalás adatai:",
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
                                <div style="font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.06em;opacity:.85;">Apartman foglalás</div>
                                <h1 style="margin:8px 0 0;font-size:24px;line-height:1.25;">{H(headline)}</h1>
                            </div>
                            <div style="padding:24px;">
                                <p style="margin:0 0 18px;font-size:15px;line-height:1.6;color:#374151;">{BuildMultilineHtml(intro)}</p>
                                {detailsHtml}
                                <p style="margin:22px 0 0;font-size:12px;line-height:1.5;color:#6b7280;">Ez egy automatikusan küldött email, kérlek ne válaszolj rá közvetlenül.</p>
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
                BuildDetailRow("Érkezés", ReservationNotificationTemplateRenderer.FormatDate(reservation.StartTIme)),
                BuildDetailRow("Távozás", ReservationNotificationTemplateRenderer.FormatDate(reservation.EndTime)),
                BuildDetailRow("Vendégek száma", reservation.PearsonCount.ToString(CultureInfo.InvariantCulture)),
                BuildDetailRow("Végösszeg", ReservationNotificationTemplateRenderer.FormatPrice(reservation.TotalPrice))
            };

            if (includeGuestContact)
            {
                rows.Add(BuildDetailRow("Foglalás neve", reservation.Name));
                rows.Add(BuildDetailRow("Telefon", reservation.PhoneNumber));
                rows.Add(BuildDetailRow("Email", reservation.Email));
            }

            if (!string.IsNullOrWhiteSpace(reservation.Description))
            {
                rows.Add(BuildDetailRow("Megjegyzés", reservation.Description));
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

        private static string BuildMultilineHtml(string? value)
        {
            var lines = (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n');

            return string.Join("<br>", lines.Select(H));
        }
    }
}
