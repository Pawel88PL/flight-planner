using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task AddToRoleAsync(User user, string role);
        Task<bool> CheckUserRoleAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<User?> FindByIdAsync(string userId);
        Task<User> FindByNameAsync(string email);
        Task<string> GenerateJwtTokenForUser(User user);
        Task<string> GenerateTwoFactorTokenAsync(User user);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<SignInResult> PasswordSignInAsync(string username, string password);
        Task<IdentityResult> RegisterNewUserAsync(Register userRegisterData);
        Task SignOutAsync();
        Task<bool> UpdateUserAsync(UpdateUser updateUser);
        Task<bool> VerifyTwoFactorTokenAsync(User user, string token);
    }
}