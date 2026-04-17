using ApartManBackend.RequestModels.RoomPriceTier;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomPriceTierController : ControllerBase
    {
        private readonly ResourceAuthService _resourceAuthService;
        private readonly RoomPriceTierService _roomPriceTierService;

        public RoomPriceTierController(ResourceAuthService resourceAuthService, RoomPriceTierService roomPriceTierService)
        {
            _resourceAuthService = resourceAuthService;
            _roomPriceTierService = roomPriceTierService;
        }

        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RoomPriceTierUpdateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomPriceTierId!.Value, ResourceObjectType.RoomPriceTier, ct))
            {
                return Forbid();
            }

            var result = await _roomPriceTierService.UpdateAsync(request, ct);
            if (result is null)
            {
                return NotFound("Nincs ilyen ar sav.");
            }

            return Ok(result);
        }
    }
}
