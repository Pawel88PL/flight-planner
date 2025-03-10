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
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthService(
            ApplicationDbContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Nie znaleziono użytkownika." });
            }

            return await _userManager.DeleteAsync(user);
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

                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    DateAdded = user.DateAdded,
                    IsActive = user.IsActive,
                    TermsAccepted = user.TermsAccepted,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                });

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
            };

            var result = await _userManager.CreateAsync(newUser, userRegisterData.Password!);

            await _userManager.AddToRoleAsync(newUser, "Pilot");

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
    }
}