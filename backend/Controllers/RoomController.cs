using ApartManBackend.Services;
using ApartManBackend.StaticMambers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ResourceAuthService _resourceAuthService;
        private readonly RoomSercie _roomSercie;

        public RoomController(ResourceAuthService resourceAuthService, RoomSercie roomSercie)
        {
            _resourceAuthService = resourceAuthService;
            _roomSercie = roomSercie;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RequestModels.Room.RoomCreateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.ApartmanId, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
            {
                return Forbid();
            }

            await _roomSercie.CreateAsync(request, ct);
            return Ok("Apartam Létrehozva");
        }

        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RequestModels.Room.RoomUpdateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomId!.Value, StaticMambers.Enums.ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            if (request.ApartmanId.HasValue)
            {
                if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.ApartmanId.Value, StaticMambers.Enums.ResourceObjectType.Apartman, ct))
                {
                    return Forbid();
                }
            }

            var result = await _roomSercie.UpdateAsync(request, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen szoba");
            }

            return Ok("Apartam Frissítve");
        }

        [HttpGet]
        [Route("{roomId:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int roomId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(roomId, StaticMambers.Enums.ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var room = await _roomSercie.GetAsync(roomId, ct);
            if (room == null)
            {
                return NotFound("Nincs ilyen szoba");
            }

            return Ok(room);
        }

        [HttpGet]
        [Route("{roomGuidId:guid}")]
        public async Task<IActionResult> GetByGuidId(Guid roomGuidId, CancellationToken ct)
        {
            var room = await _roomSercie.GetByGuidIdAsync(roomGuidId, ct);
            if (room == null)
            {
                return NotFound("Nincs ilyen szoba");
            }

            return Ok(room);
        }

        [HttpPost]
        [Route("GetAll")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll([FromBody] RequestModels.Room.RoomFillter filter, CancellationToken ct)
        {
            var rooms = await _roomSercie.GetAllAsync(filter, ct);
            return Ok(rooms);
        }

        [HttpPost]
        [Route("GetAllUser")]
        [Authorize]
        public async Task<IActionResult> GetAllUser([FromBody] RequestModels.Room.RoomFillter filter, CancellationToken ct)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Forbid();
            }

            filter.UserId = userId.Value;
            var rooms = await _roomSercie.GetAllAsync(filter, ct);
            return Ok(rooms);
        }

        [HttpDelete]
        [Route("{roomId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int roomId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(roomId, StaticMambers.Enums.ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _roomSercie.DeleteAsync(roomId, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen szoba");
            }

            return Ok("Szoba törölve");
        }
    }
}
