using ApartManBackend.RequestModels.AgePriceTier;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgePriceTierController : ControllerBase
    {
        private readonly AgePriceTierService _agePriceTierService;
        private readonly ResourceAuthService _resourceAuthService;

        public AgePriceTierController(AgePriceTierService agePriceTierService, ResourceAuthService resourceAuthService)
        {
            _agePriceTierService = agePriceTierService;
            _resourceAuthService = resourceAuthService;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AgePriceTierCreateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomId, ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _agePriceTierService.CreateAsync(request, ct);
            return Ok(result);
        }

        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] AgePriceTierUpdateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.AgePriceTierId!.Value, ResourceObjectType.AgePriceTier, ct))
            {
                return Forbid();
            }

            if (request.RoomId.HasValue &&
                !await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomId.Value, ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _agePriceTierService.UpdateAsync(request, ct);
            if (result is null)
            {
                return NotFound("Nincs ilyen eletkor ar sav.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{agePriceTierId:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int agePriceTierId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(agePriceTierId, ResourceObjectType.AgePriceTier, ct))
            {
                return Forbid();
            }

            var result = await _agePriceTierService.GetAsync(agePriceTierId, ct);
            if (result is null)
            {
                return NotFound("Nincs ilyen eletkor ar sav.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("Room/{roomId:int}")]
        [Authorize]
        public async Task<IActionResult> GetAllByRoom(int roomId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(roomId, ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _agePriceTierService.GetAllByRoomAsync(roomId, ct);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{agePriceTierId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int agePriceTierId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(agePriceTierId, ResourceObjectType.AgePriceTier, ct))
            {
                return Forbid();
            }

            var result = await _agePriceTierService.DeleteAsync(agePriceTierId, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen eletkor ar sav.");
            }

            return Ok("Eletkor ar sav torolve.");
        }
    }
}
