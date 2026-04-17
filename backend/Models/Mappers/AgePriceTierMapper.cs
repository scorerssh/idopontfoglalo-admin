using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.AgePriceTier;
using ApartManBackend.ResponseModel.AgePriceTier;
using ApartManBackend.StaticMambers;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class AgePriceTierMapper : Profile
    {
        public AgePriceTierMapper()
        {
            CreateMap<AgePriceTierCreateRequest, AgePriceTier>()
                .ForMember(dest => dest.Room, opt => opt.Ignore());

            CreateMap<AgePriceTierUpdateRequest, AgePriceTier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opts =>
                {
                    opts.PreCondition((AgePriceTierUpdateRequest src) => StaticHelpers.PatchPreConditionCheck(src, opts.DestinationMember.Name));
                    opts.Condition((src, dest, srcMember) => StaticHelpers.PatchPreConditionCheck(srcMember));
                });

            CreateMap<AgePriceTier, AgePriceTierResponse>();
        }
    }
}
