using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.Room;
using ApartManBackend.ResponseModel.Room;
using ApartManBackend.StaticMambers;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class RoomMapper:Profile
    {
        public RoomMapper()
        {
            //Create
            CreateMap<RoomCreateRequest, Room>();
            //Update
            CreateMap<RoomUpdateRequest, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Apartman, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore())
                .ForAllMembers(opts =>
                {
                    opts.PreCondition((RoomUpdateRequest src) => StaticHelpers.PatchPreConditionCheck(src, opts.DestinationMember.Name));
                    opts.Condition((src, dest, srcMember) => StaticHelpers.PatchPreConditionCheck(srcMember));
                });
            //Response
            CreateMap<Room, RoomResponse>()
                .ForMember(d => d.BindedApartmanName, o => o.MapFrom(src => src.Apartman.Name))
                .ForMember(d => d.GuidId, o => o.MapFrom(src => src.GuidId))
                .ForMember(d => d.RoomPriceTiers, o => o.MapFrom(src => src.RoomPriceTiers))
                .ForMember(d => d.AgePriceTiers, o => o.MapFrom(src => src.AgePriceTiers));
            CreateMap<Room, RoomPublicResponse>();
         
        }
    }
}
