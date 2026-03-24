using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.Apartman;
using ApartManBackend.ResponseModel.Apartman;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class ApartmanMapper:Profile
    {
        public ApartmanMapper()
        {
            //create
            CreateMap<ApartmanCreateRequest, Apartman>();

            //update
            CreateMap<ApartmanUpdateRequest, Apartman>()
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Rooms, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //response
            CreateMap<Apartman, ApartmanResponse>()
               .ForMember(d => d.Useres, opt => opt.MapFrom(src => src.Users));
            CreateMap<Apartman, ApartmanWithRoomsResponse>()
                .ForMember(d => d.Rooms, o => o.MapFrom(src => src.Rooms))
                .ForMember(d => d.Users, o => o.MapFrom(src => src.Users));
        }
    }
}
