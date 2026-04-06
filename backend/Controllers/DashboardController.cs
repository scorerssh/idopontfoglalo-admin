using ApartManBackend.Services;
using ApartManBackend.StaticMambers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Forbid();
            }

            var dashboard = await _dashboardService.GetAsync(userId.Value, ct);
            return Ok(dashboard);
        }
    }
}
