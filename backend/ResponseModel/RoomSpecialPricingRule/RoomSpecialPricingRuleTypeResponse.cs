using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.ResponseModel.RoomSpecialPricingRule
{
    public class RoomSpecialPricingRuleTypeResponse
    {
        public SpecialPricingRuleType RuleType { get; set; }
        public string Description { get; set; } = null!;
    }
}
