using ApartManBackend.Models.DbModels.Models;
using System.Globalization;

namespace ApartManBackend.Services
{
    public static class ReservationNotificationTemplateRenderer
    {
        public const string DefaultGuestEmailIntroTemplate =
            "Köszönjük a foglalást. Az alábbi adatokkal rögzítettük a foglalásodat a(z) {apartmanName} apartmanhoz.";

        public const string DefaultGuestSmsTemplate =
            "Kedves {guestName}! Foglalásodat rögzítettük: {apartmanName}, {roomName}, érkezés: {startDate}, távozás: {endDate}. Végösszeg: {totalPrice}.";

        public static string RenderGuestEmailIntro(
            ApartmanSmtpSetting smtpSetting,
            Reservation reservation,
            Apartman apartman)
        {
            var template = string.IsNullOrWhiteSpace(smtpSetting.GuestEmailIntroTemplate)
                ? DefaultGuestEmailIntroTemplate
                : smtpSetting.GuestEmailIntroTemplate;

            return Render(template, reservation, apartman);
        }

        public static string RenderGuestSms(
            ApartmanSmtpSetting smtpSetting,
            Reservation reservation,
            Apartman apartman)
        {
            var template = string.IsNullOrWhiteSpace(smtpSetting.GuestSmsTemplate)
                ? DefaultGuestSmsTemplate
                : smtpSetting.GuestSmsTemplate;

            return Render(template, reservation, apartman);
        }

        public static string FormatDate(DateOnly date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string FormatPrice(decimal value)
        {
            return $"{value.ToString("N0", CultureInfo.GetCultureInfo("hu-HU")).Replace('\u00A0', ' ')} Ft";
        }

        private static string Render(string template, Reservation reservation, Apartman apartman)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["guestName"] = reservation.Name,
                ["apartmanName"] = apartman.Name,
                ["roomName"] = reservation.Room.Name,
                ["startDate"] = FormatDate(reservation.StartTIme),
                ["endDate"] = FormatDate(reservation.EndTime),
                ["guestCount"] = reservation.PearsonCount.ToString(CultureInfo.InvariantCulture),
                ["totalPrice"] = FormatPrice(reservation.TotalPrice),
                ["phoneNumber"] = reservation.PhoneNumber,
                ["email"] = reservation.Email,
                ["description"] = reservation.Description ?? string.Empty,
                ["nev"] = reservation.Name,
                ["apartman"] = apartman.Name,
                ["szoba"] = reservation.Room.Name,
                ["erkezes"] = FormatDate(reservation.StartTIme),
                ["tavozas"] = FormatDate(reservation.EndTime),
                ["vendegek"] = reservation.PearsonCount.ToString(CultureInfo.InvariantCulture),
                ["vegosszeg"] = FormatPrice(reservation.TotalPrice)
            };

            var rendered = template;
            foreach (var value in values)
            {
                rendered = rendered.Replace($"{{{value.Key}}}", value.Value, StringComparison.OrdinalIgnoreCase);
            }

            return NormalizeLineEndings(rendered).Trim();
        }

        private static string NormalizeLineEndings(string value)
        {
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
