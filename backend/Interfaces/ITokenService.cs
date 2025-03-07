using backend.Models;

namespace backend.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenForUser(User user);
    }
}