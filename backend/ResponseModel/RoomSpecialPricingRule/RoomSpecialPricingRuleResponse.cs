using ApartManBackend.StaticMambers;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.ResponseModel.RoomSpecialPricingRule
{
    public class RoomSpecialPricingRuleResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public SpecialPricingRuleType RuleType { get; set; }
        public string Description => SpecialPricingRuleDescriptions.GetDescription(RuleType);
        public int Priority { get; set; }
        public bool Active { get; set; }
    }
}
