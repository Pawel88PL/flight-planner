using System.Security.Claims;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public TokenController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("Brak identyfikatora użytkownika w zapytaniu.");
            }

            var user = await _authService.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            var token = _tokenService.GenerateJwtTokenForUser(user);

            return Ok(new { Token = token });
        }
    }
}