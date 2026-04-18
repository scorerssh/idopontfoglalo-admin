using ApartManBackend.Models.DbModels.Models;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class WeekendSurchargeRuleCalculator : IRoomSpecialPricingRuleCalculator
    {
        private const decimal SurchargeRate = 0.20m;

        public SpecialPricingRuleType RuleType => SpecialPricingRuleType.WeekendSurcharge;

        public decimal CalculateAdjustment(ReservationPricingContext context, RoomSpecialPricingRule rule)
        {
            var weekendNights = context.NightsInStay.Count(date =>
                date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday);

            return context.BasePricePerNight * weekendNights * SurchargeRate;
        }
    }
}
