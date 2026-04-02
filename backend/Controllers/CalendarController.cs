using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly RoomCalendarService _roomCalendarService;

        public CalendarController(RoomCalendarService roomCalendarService)
        {
            _roomCalendarService = roomCalendarService;
        }

        [HttpGet("{roomGuid:guid}")]
        [HttpGet("{roomGuid:guid}.ics")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomCalendar(Guid roomGuid, CancellationToken ct)
        {
            var calendar = await _roomCalendarService.GetCalendarFileAsync(roomGuid, ct);
            if (calendar is null)
            {
                return NotFound("Nincs ilyen naptar fajl");
            }

            return File(calendar.Content, calendar.ContentType, calendar.FileName);
        }
    }
}
