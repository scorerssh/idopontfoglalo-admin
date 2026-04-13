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
            .ForAllMembers(opts =>
              opts.Condition((src, dest, srcMember) =>StaticHelpers.PatchPreConditionCheck(srcMember)));
            //Response
            CreateMap<Room, RoomResponse>()
                .ForMember(d => d.BindedApartmanName, o => o.MapFrom(src => src.Apartman.Name))
                .ForMember(d => d.GuidId, o => o.MapFrom(src => src.GuidId));
            CreateMap<Room, RoomPublicResponse>();
         
        }
    }
}
