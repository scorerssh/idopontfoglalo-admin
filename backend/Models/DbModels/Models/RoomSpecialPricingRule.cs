using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Models.DbModels.Models
{
    public class RoomSpecialPricingRule:BaseDbModel
    {
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public SpecialPricingRuleType RuleType { get; set; }

        public int Priority { get; set; }

        public bool Active { get; set; } = true;
    }
}
