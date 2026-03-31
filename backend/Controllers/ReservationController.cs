using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.Services;
using ApartManBackend.StaticMambers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;
        private readonly ResourceAuthService _resourceAuthService;

        public ReservationController(ReservationService reservationService, ResourceAuthService resourceAuthService)
        {
            _reservationService = reservationService;
            _resourceAuthService = resourceAuthService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]ReservationCreateRequest request,CancellationToken ct)
        {
            await _reservationService.CreateAsync(request, ct);
            return Ok();
        }

        [HttpPost]
        [Route("GetAllAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin([FromBody] ReservationFillter filter, CancellationToken ct)
        {
            var reservations = await _reservationService.GetAllAsync(filter, ct);
            return Ok(reservations);
        }
        [HttpPost]
        [Route("GetAllUser")]
        [Authorize]

        public async Task<IActionResult> GetAllUser([FromBody] ReservationFillter filter, CancellationToken ct)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Forbid();
            }

            filter.UserId = userId.Value;
            var reservations = await _reservationService.GetAllAsync(filter, ct);
            return Ok(reservations);
        }

        [HttpGet]
        [Route("{Reservationid:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int Reservationid,CancellationToken ct)
        {
            if(!await _resourceAuthService.CheckUserAccessForResourceAsync(Reservationid,StaticMambers.Enums.ResourceObjectType.Reservation,ct))
            {
                return Forbid();
            }
            var resault = await _reservationService.GetAsnyc(Reservationid, ct);
            if (resault == null)
            {
                return NotFound("Nincs ilyen foglalás");
            }

            return Ok(resault);

        }
    }
}
