using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }


        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount(string userId, string token)
        {
            var user = await _authService.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            var result = await _authService.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Redirect("https://zion.sigid.pl/login");
            }
            else
            {
                return BadRequest("Nie udało się aktywować konta.");
            }
        }

        [Authorize]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login userLoginData)
        {
            if (!ModelState.IsValid || userLoginData.UserName == null || userLoginData.Password == null)
            {
                return BadRequest(ModelState);
            }

            // Próba logowania
            var signInResult = await _authService.PasswordSignInAsync(userLoginData.UserName!, userLoginData.Password!);

            // Pobierz użytkownika
            var user = await _authService.FindByNameAsync(userLoginData.UserName);
            if (user == null)
            {
                var message = "Podany identyfikator lub hasło są nieprawidłowe.";
                return NotFound(new { message });
            }

            // Sprawdź czy użytkownik jest administratorem lub operatorem
            var isAdminOrOperator = await _authService.CheckUserRoleAsync(user);
            if (isAdminOrOperator)
            {
                if (!user.EmailConfirmed)
                {
                    var message = "Konto nie jest aktywne. Sprawdź swoją skrzynkę mailową w celu aktywacji konta.";
                    return Unauthorized(new { message });
                }

                if (!user.IsActive)
                {
                    var message = "Konto jest zablokowane. Skontaktuj się z administratorem.";
                    return Unauthorized(new { message });
                }
            }

            // Obsługa różnych wyników logowania
            if (signInResult.RequiresTwoFactor)
            {
                // Użytkownik musi przejść przez drugi etap logowania (2FA)
                if (user.TwoFactorEnabled)
                {
                    // Generowanie kodu 2FA i wysyłanie do użytkownika
                    await _authService.GenerateTwoFactorTokenAsync(user);
                    return Ok(new { Message = "2FA", Id = user.Id });
                }
            }
            else if (!signInResult.Succeeded)
            {
                var message = "Podany identyfikator lub hasło są nieprawidłowe.";
                return Unauthorized(new { message });
            }

            var token = _authService.GenerateJwtTokenForUser(user);
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return Ok();
        }

        //[Authorize(Roles = "Administrator")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register userRegisterData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userRegisterData.Email == null)
            {
                return BadRequest("Brak nazwy użytkownika w zapytaniu.");
            }

            var result = await _authService.RegisterNewUserAsync(userRegisterData);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUser updateUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updateResult = await _authService.UpdateUserAsync(updateUser);
                if (!updateResult)
                {
                    return BadRequest("Nie udało się zaktualizować użytkownika.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Wystąpił błąd podczas aktualizacji użytkownika.");
                return StatusCode(500, "Wystąpił błąd podczas aktualizacji użytkownika.");
            }
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorRequest request)
        {
            var user = await _authService.FindByIdAsync(request.Id!);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            var isValid = await _authService.VerifyTwoFactorTokenAsync(user, request.Code!);
            if (!isValid)
            {
                return Unauthorized(new { message = "Nieprawidłowy kod 2FA." });
            }

            var token = _authService.GenerateJwtTokenForUser(user);
            return Ok(new { Token = token });
        }
    }
}