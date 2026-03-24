using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.RequestModels.User;
using ApartManBackend.ResponseModel.User;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class UserMapper:Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.ApartmanIds, opt => opt.MapFrom(src => src.Apartmans.Select(a => a.Id).ToList()));

            CreateMap<UserCreateRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role ?? StaticMambers.Enums.UserRole.User));

            CreateMap<UserUpdateRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt =>
                {
                    opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Password));
                    opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password));
                })
                .ForMember(dest => dest.Role, opt =>
                {
                    opt.PreCondition(src => src.Role.HasValue);
                    opt.MapFrom(src => src.Role!.Value);
                });
         
           



        }
    }
}
