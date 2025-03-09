using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // <-- Konieczne do dynamicznej zmiany sortowania

namespace backend.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;

        public UsersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedUsers> GetUsersPaged(PagedRequest request)
        {
            var query = from user in _context.Users.AsNoTracking()
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        select new
                        {
                            User = user,
                            RoleName = role.Name
                        };

            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                query = query.Where(c =>
                    c.User.FirstName!.Contains(request.SearchQuery) ||
                    c.User.LastName!.Contains(request.SearchQuery) ||
                    c.User.Email!.Contains(request.SearchQuery));
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && request.SortColumn == "role")
            {
                var sortExpression = $"RoleName {(request.SortDirection == "desc" ? "descending" : "ascending")}";
                query = query.OrderBy(sortExpression);
            }
            else if (!string.IsNullOrEmpty(request.SortColumn))
            {
                var sortExpression = $"User.{request.SortColumn} {(request.SortDirection == "desc" ? "descending" : "ascending")}";
                query = query.OrderBy(sortExpression);
            }

            int totalRecords = await query.CountAsync();

            var usersWithRoles = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var usersDto = usersWithRoles.Select(item => new UserDTO
            {
                Id = item.User.Id.ToString(),
                FirstName = item.User.FirstName,
                LastName = item.User.LastName,
                Email = item.User.Email,
                PhoneNumber = item.User.PhoneNumber,
                Role = item.RoleName,
                IsActive = item.User.IsActive,
                DateAdded = item.User.DateAdded,
                LastSuccessfulLogin = item.User.LastSuccessfulLogin
            }).ToList();

            return new PagedUsers
            {
                TotalRecords = totalRecords,
                Data = usersDto
            };
        }
    }
}