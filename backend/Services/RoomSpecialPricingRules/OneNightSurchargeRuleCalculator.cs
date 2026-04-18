using ApartManBackend.Models.DbModels.Models;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class OneNightSurchargeRuleCalculator : IRoomSpecialPricingRuleCalculator
    {
        private const decimal SurchargeRate = 0.30m;

        public SpecialPricingRuleType RuleType => SpecialPricingRuleType.OneNightSurcharge;

        public decimal CalculateAdjustment(ReservationPricingContext context, RoomSpecialPricingRule rule)
        {
            return context.Nights == 1
                ? context.BaseTotalPrice * SurchargeRate
                : 0;
        }
    }
}
