using backend.Models;

namespace backend.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenForUser(User user);
        Task<string> GenerateTwoFactorTokenAsync(User user);
        Task<bool> VerifyTwoFactorTokenAsync(User user, string token);
    }
}