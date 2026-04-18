using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.RequestModels.RoomSpecialPricingRule
{
    public class RoomSpecialPricingRuleUpdateRequest
    {
        public int? RoomSpecialPricingRuleId { get; set; }
        public int? RoomId { get; set; }
        public SpecialPricingRuleType? RuleType { get; set; }
        public int? Priority { get; set; }
        public bool? Active { get; set; }
    }
}
