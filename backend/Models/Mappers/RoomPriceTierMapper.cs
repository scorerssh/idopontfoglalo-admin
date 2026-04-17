using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.RoomPriceTier;
using ApartManBackend.ResponseModel.RoomPriceTier;
using ApartManBackend.StaticMambers;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class RoomPriceTierMapper : Profile
    {
        public RoomPriceTierMapper()
        {
            CreateMap<RoomPriceTierUpdateRequest, RoomPriceTier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RoomId, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.GuestCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opts =>
                {
                    opts.PreCondition((RoomPriceTierUpdateRequest src) => StaticHelpers.PatchPreConditionCheck(src, opts.DestinationMember.Name));
                    opts.Condition((src, dest, srcMember) => StaticHelpers.PatchPreConditionCheck(srcMember));
                });

            CreateMap<RoomPriceTier, RoomPriceTierResponse>();
        }
    }
}
