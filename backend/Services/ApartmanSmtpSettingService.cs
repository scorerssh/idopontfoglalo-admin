using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.ApartmanSmtpSetting;
using ApartManBackend.ResponseModel.ApartmanSmtpSetting;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class ApartmanSmtpSettingService
    {
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;

        public ApartmanSmtpSettingService(ApartmanDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public Task<ApartmanSmtpSettingResponse?> GetAsync(int apartmanId, CancellationToken ct)
        {
            return _db.ApartmanSmtpSettings
                .AsNoTracking()
                .Where(x => x.ApartmanId == apartmanId)
                .ProjectTo<ApartmanSmtpSettingResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ApartmanSmtpSettingResponse> UpsertAsync(ApartmanSmtpSettingUpsertRequest request, CancellationToken ct)
        {
            var apartmanId = request.ApartmanId!.Value;
            var smtpSetting = await _db.ApartmanSmtpSettings
                .FirstOrDefaultAsync(x => x.ApartmanId == apartmanId, ct);

            if (smtpSetting is null)
            {
                smtpSetting = new ApartmanSmtpSetting
                {
                    ApartmanId = apartmanId
                };

                await _db.ApartmanSmtpSettings.AddAsync(smtpSetting, ct);
            }

            smtpSetting.Host = request.Host!.Trim();
            smtpSetting.Port = request.Port!.Value;
            smtpSetting.UserName = string.IsNullOrWhiteSpace(request.UserName) ? null : request.UserName.Trim();
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                smtpSetting.Password = request.Password;
            }

            smtpSetting.SenderEmail = request.SenderEmail!.Trim();
            smtpSetting.SenderName = string.IsNullOrWhiteSpace(request.SenderName) ? null : request.SenderName.Trim();
            smtpSetting.UseSsl = request.UseSsl ?? false;
            smtpSetting.IsEnabled = request.IsEnabled ?? true;

            await _db.SaveChangesAsync(ct);
            return _mapper.Map<ApartmanSmtpSettingResponse>(smtpSetting);
        }

        public async Task<bool> DeleteAsync(int apartmanId, CancellationToken ct)
        {
            var affectedRows = await _db.ApartmanSmtpSettings
                .Where(x => x.ApartmanId == apartmanId)
                .ExecuteDeleteAsync(ct);

            return affectedRows > 0;
        }
    }
}
