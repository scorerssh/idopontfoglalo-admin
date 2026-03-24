using ApartManBackend.RequestModels.User;
using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request, CancellationToken ct)
        {
            await _userService.CreateAsync(request, ct);
            return Ok("Felhasznalo letrehozva");
        }

        [HttpPatch]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequest request, CancellationToken ct)
        {
            var result = await _userService.UpdateAsync(request, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen felhasznalo");
            }

            return Ok("Felhasznalo frissitve");
        }

        [HttpGet]
        [Route("{userId:int}")]
        public async Task<IActionResult> Get(int userId, CancellationToken ct)
        {
            var user = await _userService.GetAsync(userId, ct);
            if (user == null)
            {
                return NotFound("Nincs ilyen felhasznalo");
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] UserFilter filter, CancellationToken ct)
        {
            var users = await _userService.GetAllAsync(filter, ct);
            return Ok(users);
        }

        [HttpDelete]
        [Route("{userId:int}")]
        public async Task<IActionResult> Delete(int userId, CancellationToken ct)
        {
            var result = await _userService.DeleteAsync(userId, ct);
            if (!result)
            {
                return NotFound("Nincs ilyen felhasznalo");
            }

            return Ok("Felhasznalo torolve");
        }
    }
}
