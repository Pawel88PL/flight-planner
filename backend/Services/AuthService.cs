using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailService emailService,
            SignInManager<User> signInManager,
            UserManager<User> userManager
            )
        {
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> CheckUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Administrator") || roles.Contains("Operator"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            var userToConfirm = await _userManager.FindByIdAsync(user.Id);
            if (userToConfirm == null)
            {
                throw new ArgumentException("Użytkownik nie istnieje.");
            }

            userToConfirm.IsActive = true;
            await _userManager.UpdateAsync(userToConfirm);

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User> FindByNameAsync(string email)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                return null!;
            }

            return user;
        }

        private static string GenerateRandomPasword()
        {

            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);
        }

        private static string GenerateRandomUserId()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        }

        public async Task<string> GenerateJwtTokenForUser(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not set in the configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.FirstName ?? string.Empty),
                new (ClaimTypes.Surname, user.LastName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: _configuration["Jwt:Expires"] == null
                    ? DateTime.Now.AddMinutes(30)
                    : DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expires"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Metoda generująca i wysyłająca kod 2FA
        public async Task<string> GenerateTwoFactorTokenAsync(User user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            await _emailService.SendTwoFactorCodeEmail(user.Id, token);
            return token;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            if (_context.Users == null)
            {
                throw new ArgumentNullException(nameof(_context.Users));
            }

            var users = await _context.Users.ToListAsync();

            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Administrator") || roles.Contains("Operator"))
                {
                    userDTOs.Add(new UserDTO
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        DateAdded = user.DateAdded.ToString("o"),
                        IsActive = user.IsActive,
                        TermsAccepted = user.TermsAccepted,
                        Role = roles.FirstOrDefault() ?? string.Empty,
                    });
                }
            }

            return userDTOs.OrderBy(u => u.DateAdded).ThenBy(u => u.LastName).ThenBy(u => u.FirstName);
        }

        public async Task<SignInResult> PasswordSignInAsync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: true);
            return result;
        }

        public async Task<IdentityResult> RegisterNewUserAsync(Register userRegisterData)
        {

            var user = await _userManager.FindByNameAsync(userRegisterData.Email!);

            if (user != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Użytkownik o podanym adresie e-mail już istnieje." });
            }

            var newUser = new User
            {
                FirstName = userRegisterData.FirstName!,
                LastName = userRegisterData.LastName!,
                UserName = userRegisterData.Email!,
                Email = userRegisterData.Email!,
                DateAdded = DateTime.Now,
                IsActive = true,
                TwoFactorEnabled = userRegisterData.Role == "Admin"
            };

            var result = await _userManager.CreateAsync(newUser, userRegisterData.Password!);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            await _emailService.SendActivationEmail(newUser.Id, token);

            await AddToRoleAsync(newUser, userRegisterData.Role!);

            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> UpdateUserAsync(UpdateUser updateUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == updateUser.Id)
                    ?? throw new ArgumentNullException(nameof(updateUser));

                if (user == null) return false;

                // Aktualizacja danych użytkownika
                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
                user.UserName = updateUser.Email;
                user.Email = updateUser.Email;
                user.IsActive = updateUser.IsActive;

                // Aktualizacja hasła użytkownika
                if (!string.IsNullOrEmpty(updateUser.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPassResult = await _userManager.ResetPasswordAsync(user, token, updateUser.NewPassword);
                    if (!resetPassResult.Succeeded)
                    {
                        // Logowanie błędów resetowania hasła
                        foreach (var error in resetPassResult.Errors)
                        {
                            Log.Error($"Error resetting password for user with ID {user.Id}: {error.Description}");
                        }
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                // Aktualizacja roli użytkownika
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        // Logowanie błędów usuwania ról
                        foreach (var error in removeRolesResult.Errors)
                        {
                            Log.Error($"Error removing roles for user with ID {user.Id}: {error.Description}");
                        }
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                // Dodanie nowej roli
                if (!string.IsNullOrEmpty(updateUser.Role))
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, updateUser.Role);
                    if (!addRoleResult.Succeeded)
                    {
                        // Logowanie błędów dodawania ról
                        foreach (var error in addRoleResult.Errors)
                        {
                            Log.Error($"Error adding role for user with ID {user.Id}: {error.Description}");
                        }
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, $"Error updating user with ID {updateUser.Id}");
                return false;
            }
        }

        // Metoda do weryfikacji kodu 2FA
        public async Task<bool> VerifyTwoFactorTokenAsync(User user, string token)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, token);
        }
    }
}