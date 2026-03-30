using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]ReservationCreateRequest request,CancellationToken ct)
        {
            await _reservationService.CreateAsync(request, ct);
            return Ok();
        }
    }
}
