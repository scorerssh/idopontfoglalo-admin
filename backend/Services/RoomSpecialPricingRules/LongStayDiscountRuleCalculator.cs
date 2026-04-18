using ApartManBackend.Models.DbModels.Models;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class LongStayDiscountRuleCalculator : IRoomSpecialPricingRuleCalculator
    {
        private const int MinimumNights = 7;
        private const decimal DiscountRate = 0.10m;

        public SpecialPricingRuleType RuleType => SpecialPricingRuleType.LongStayDiscount;

        public decimal CalculateAdjustment(ReservationPricingContext context, RoomSpecialPricingRule rule)
        {
            return context.Nights >= MinimumNights
                ? -context.BaseTotalPrice * DiscountRate
                : 0;
        }
    }
}
