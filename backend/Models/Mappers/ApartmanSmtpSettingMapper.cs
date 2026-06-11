using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.ResponseModel.ApartmanSmtpSetting;
using AutoMapper;

namespace ApartManBackend.Models.Mappers
{
    public class ApartmanSmtpSettingMapper : Profile
    {
        public ApartmanSmtpSettingMapper()
        {
            CreateMap<ApartmanSmtpSetting, ApartmanSmtpSettingResponse>()
                .ForMember(x => x.HasPassword, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Password)));
        }
    }
}
