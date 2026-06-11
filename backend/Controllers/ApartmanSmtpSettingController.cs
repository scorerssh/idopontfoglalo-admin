using ApartManBackend.RequestModels.ApartmanSmtpSetting;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ApartmanSmtpSettingController : ControllerBase
    {
        private readonly ApartmanSmtpSettingService _apartmanSmtpSettingService;

        public ApartmanSmtpSettingController(ApartmanSmtpSettingService apartmanSmtpSettingService)
        {
            _apartmanSmtpSettingService = apartmanSmtpSettingService;
        }

        [HttpGet("{apartmanId:int}")]
        public async Task<IActionResult> Get(int apartmanId, CancellationToken ct)
        {
            var smtpSetting = await _apartmanSmtpSettingService.GetAsync(apartmanId, ct);
            if (smtpSetting is null)
            {
                return NotFound("Nincs SMTP beallitas ehhez az apartmanhoz.");
            }

            return Ok(smtpSetting);
        }

        [HttpPut]
        public async Task<IActionResult> Upsert([FromBody] ApartmanSmtpSettingUpsertRequest request, CancellationToken ct)
        {
            var smtpSetting = await _apartmanSmtpSettingService.UpsertAsync(request, ct);
            return Ok(smtpSetting);
        }

        [HttpDelete("{apartmanId:int}")]
        public async Task<IActionResult> Delete(int apartmanId, CancellationToken ct)
        {
            var deleted = await _apartmanSmtpSettingService.DeleteAsync(apartmanId, ct);
            if (!deleted)
            {
                return NotFound("Nincs SMTP beallitas ehhez az apartmanhoz.");
            }

            return Ok("SMTP beallitas torolve");
        }
    }
}
