using ApartManBackend.RequestModels.RoomSpecialPricingRule;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomSpecialPricingRuleController : ControllerBase
    {
        private readonly ResourceAuthService _resourceAuthService;
        private readonly RoomSpecialPricingRuleService _roomSpecialPricingRuleService;

        public RoomSpecialPricingRuleController(
            ResourceAuthService resourceAuthService,
            RoomSpecialPricingRuleService roomSpecialPricingRuleService)
        {
            _resourceAuthService = resourceAuthService;
            _roomSpecialPricingRuleService = roomSpecialPricingRuleService;
        }

        [HttpGet]
        [Route("Types")]
        [Authorize]
        public IActionResult GetTypes()
        {
            return Ok(_roomSpecialPricingRuleService.GetRuleTypes());
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RoomSpecialPricingRuleCreateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomId, ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _roomSpecialPricingRuleService.CreateAsync(request, ct);
            return Ok(result);
        }

        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RoomSpecialPricingRuleUpdateRequest request, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomSpecialPricingRuleId!.Value, ResourceObjectType.RoomSpecialPricingRule, ct))
            {
                return Forbid();
            }

            if (request.RoomId.HasValue &&
                !await _resourceAuthService.CheckUserAccessForResourceAsync(request.RoomId.Value, ResourceObjectType.Room, ct))
            {
                return Forbid();
            }

            var result = await _roomSpecialPricingRuleService.UpdateAsync(request, ct);
            if (result is null)
            {
                return NotFound("Nincs ilyen specialis arazasi szabaly.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{roomSpecialPricingRuleId:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int roomSpecialPricingRuleId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(roomSpecialPricingRuleId, ResourceObjectType.RoomSpecialPricingRule, ct))
            {
                return Forbid();
            }

            var result = await _roomSpecialPricingRuleService.GetAsync(roomSpecialPricingRuleId, ct);
            if (result is null)
            {
                return NotFound("Nincs ilyen specialis arazasi szabaly.");
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

            var result = await _roomSpecialPricingRuleService.GetAllByRoomAsync(roomId, ct);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{roomSpecialPricingRuleId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int roomSpecialPricingRuleId, CancellationToken ct)
        {
            if (!await _resourceAuthService.CheckUserAccessForResourceAsync(roomSpecialPricingRuleId, ResourceObjectType.RoomSpecialPricingRule, ct))
            {
                return Forbid();
            }

            var result = await _roomSpecialPricingRuleService.DeleteAsync(roomSpecialPricingRuleId, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen specialis arazasi szabaly.");
            }

            return Ok("Specialis arazasi szabaly torolve.");
        }
    }
}
