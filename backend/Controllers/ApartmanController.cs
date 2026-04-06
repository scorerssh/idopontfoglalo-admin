using ApartManBackend.RequestModels.Apartman;
using ApartManBackend.Services;
using ApartManBackend.StaticMambers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmanController : ControllerBase
    {
        private readonly ResourceAuthService _resourceAuthService;
        private readonly ApartmanService _apartmanService;

        public ApartmanController(ResourceAuthService resourceAuthService, ApartmanService apartmanService)
        {
            _resourceAuthService = resourceAuthService;
            _apartmanService = apartmanService;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ApartmanCreateRequest request, CancellationToken ct)
        {
            await _apartmanService.CreateAync(request, ct);
            return Ok("Apartman letrehozva");
        }

        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ApartmanUpdateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.Id!.Value, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
            {
                return Forbid();
            }

            await _apartmanService.UpdateAsync(request, ct);
            return Ok("Apartman frissitve");
        }

        [HttpGet]
        [Route("{apartmanId:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int apartmanId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(apartmanId, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
            {
                return Forbid();
            }

            var apartman = await _apartmanService.GetAsync(apartmanId, ct);
            if (apartman == null)
            {
                return NotFound("Nincs ilyen apartman");
            }

            return Ok(apartman);
        }

        [HttpGet]
        [Route("WithRooms/{apartmanId:int}")]
        [Authorize]
        public async Task<IActionResult> GetWithRooms(int apartmanId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(apartmanId, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
            {
                return Forbid();
            }

            var apartman = await _apartmanService.GetWithRoomsAsync(apartmanId, ct);
            if (apartman == null)
            {
                return NotFound("Nincs ilyen apartman");
            }

            return Ok(apartman);
        }

        [HttpPost]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromBody] ApartmanFillter filter, CancellationToken ct)
        {
            var apartmans = await _apartmanService.GetAllAsync(filter, ct);
            return Ok(apartmans);
        }

        [HttpPost]
        [Route("GetAllUser")]
        [Authorize]
        public async Task<IActionResult> GetAllUser([FromBody] ApartmanFillter filter, CancellationToken ct)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Forbid();
            }

            filter.InUserId = userId.Value;
            var apartmans = await _apartmanService.GetAllAsync(filter, ct);
            return Ok(apartmans);
        }

        [HttpPost]
        [Route("GetAllWithRooms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllWithRooms([FromBody] ApartmanFillter filter, CancellationToken ct)
        {
            var apartmans = await _apartmanService.GetAllWithRoomsAsync(filter, ct);
            return Ok(apartmans);
        }

        [HttpDelete]
        [Route("{apartmanId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int apartmanId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(apartmanId, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
            {
                return Forbid();
            }

            var result = await _apartmanService.DeleteAsync(apartmanId, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen apartman");
            }

            return Ok("Apartman torolve");
        }
    }
}
