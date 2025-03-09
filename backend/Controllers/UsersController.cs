using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetUsersPaged([FromQuery] PagedRequest pagedRequest)
        {
            try
            {
                var users = await _usersService.GetUsersPaged(pagedRequest);
                return Ok(users);
            }
            catch (Exception e)
            {
                var message = $"Wystąpił błąd podczas pobierania użytkowników: {e.Message}";
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}