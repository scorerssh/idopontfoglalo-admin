using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.ResponseModel.Reservation;
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

            CreateMap<Reservation, ReservationResponse>();
        }
    }
}
