using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.ResponseModel.Reservation;
using ApartManBackend.StaticMambers;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class ReservationMapper:Profile
    {
        public ReservationMapper()
        {
            CreateMap<ReservationCreateRequest, Reservation>()
                .ForMember(x => x.RoomId, opt => opt.Ignore())
                .ForMember(x => x.Room, opt => opt.Ignore());

            CreateMap<ReservationUpdateRequest, Reservation>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.Room, opt => opt.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => StaticHelpers.PatchPreConditionCheck(srcMember)));

            CreateMap<Reservation, ReservationResponse>();
        }
    }
}
