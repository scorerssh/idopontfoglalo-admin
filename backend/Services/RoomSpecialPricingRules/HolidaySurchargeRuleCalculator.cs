using ApartManBackend.Models.DbModels.Models;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class HolidaySurchargeRuleCalculator : IRoomSpecialPricingRuleCalculator
    {
        private const decimal SurchargeRate = 0.25m;

        public SpecialPricingRuleType RuleType => SpecialPricingRuleType.HolidaySurcharge;

        public decimal CalculateAdjustment(ReservationPricingContext context, RoomSpecialPricingRule rule)
        {
            var holidayNights = context.NightsInStay.Count(IsHungarianPublicHoliday);

            return context.BasePricePerNight * holidayNights * SurchargeRate;
        }

        private static bool IsHungarianPublicHoliday(DateOnly date)
        {
            if (IsFixedHungarianPublicHoliday(date))
            {
                return true;
            }

            var easterSunday = GetEasterSunday(date.Year);
            return date == easterSunday.AddDays(-2) ||
                   date == easterSunday.AddDays(1) ||
                   date == easterSunday.AddDays(50);
        }

        private static bool IsFixedHungarianPublicHoliday(DateOnly date)
        {
            return (date.Month, date.Day) is
                (1, 1) or
                (3, 15) or
                (5, 1) or
                (8, 20) or
                (10, 23) or
                (11, 1) or
                (12, 25) or
                (12, 26);
        }

        private static DateOnly GetEasterSunday(int year)
        {
            var a = year % 19;
            var b = year / 100;
            var c = year % 100;
            var d = b / 4;
            var e = b % 4;
            var f = (b + 8) / 25;
            var g = (b - f + 1) / 3;
            var h = (19 * a + b - d - g + 15) % 30;
            var i = c / 4;
            var k = c % 4;
            var l = (32 + 2 * e + 2 * i - h - k) % 7;
            var m = (a + 11 * h + 22 * l) / 451;
            var month = (h + l - 7 * m + 114) / 31;
            var day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateOnly(year, month, day);
        }
    }
}
