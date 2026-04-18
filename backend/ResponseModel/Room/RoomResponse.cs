using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.ResponseModel.AgePriceTier;
using ApartManBackend.ResponseModel.RoomPriceTier;
using ApartManBackend.ResponseModel.RoomSpecialPricingRule;

namespace ApartManBackend.ResponseModel.Room
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public string? BindedApartmanName { get; set; }
        public Guid GuidId { get; set; }
        public int ApartmanId { get; set; }
        public bool Active { get; set; }

        public string? BookingConnectionUrl { get; set; }
        public string? SzallasHuConnectionUrl { get; set; }
        public List<RoomPriceTierResponse>? RoomPriceTiers { get; set; }
        public List<RoomSpecialPricingRuleResponse>? RoomSpecialPricingRules { get; set; }
        public List<AgePriceTierResponse>? AgePriceTiers { get; set; }
    }
}
