using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<User?> FindByIdAsync(string userId);
        Task<User> FindByNameAsync(string email);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<SignInResult> PasswordSignInAsync(string username, string password);
        Task<IdentityResult> RegisterNewUserAsync(Register userRegisterData);
        Task SignOutAsync();
        Task<bool> UpdateUserAsync(UpdateUser updateUser);
    }
}