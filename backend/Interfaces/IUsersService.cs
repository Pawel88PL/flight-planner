using backend.Models;

namespace backend.Interfaces
{
    public interface IUsersService
    {
        Task<PagedUsers> GetUsersPaged(PagedRequest request);
    }
}