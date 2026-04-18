using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.RoomSpecialPricingRule;
using ApartManBackend.ResponseModel.RoomSpecialPricingRule;
using ApartManBackend.StaticMambers;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class RoomSpecialPricingRuleMapper : Profile
    {
        public RoomSpecialPricingRuleMapper()
        {
            CreateMap<RoomSpecialPricingRuleCreateRequest, RoomSpecialPricingRule>()
                .ForMember(dest => dest.Room, opt => opt.Ignore());

            CreateMap<RoomSpecialPricingRuleUpdateRequest, RoomSpecialPricingRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opts =>
                {
                    opts.PreCondition((RoomSpecialPricingRuleUpdateRequest src) => StaticHelpers.PatchPreConditionCheck(src, opts.DestinationMember.Name));
                    opts.Condition((src, dest, srcMember) => StaticHelpers.PatchPreConditionCheck(srcMember));
                });

            CreateMap<RoomSpecialPricingRule, RoomSpecialPricingRuleResponse>();
        }
    }
}
