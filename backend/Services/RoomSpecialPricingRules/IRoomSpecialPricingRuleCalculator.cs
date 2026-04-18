using ApartManBackend.Models.DbModels.Models;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public interface IRoomSpecialPricingRuleCalculator
    {
        SpecialPricingRuleType RuleType { get; }
        decimal CalculateAdjustment(ReservationPricingContext context, RoomSpecialPricingRule rule);
    }
}
