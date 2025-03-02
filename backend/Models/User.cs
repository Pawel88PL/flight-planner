using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public bool TermsAccepted { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? LastSuccessfulLogin { get; set; }
    }

    public class UserDTO
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DateAdded { get; set; }
        public string? LastSuccessfulLogin { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool TermsAccepted { get; set; }
    }

    public class Login
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class Register
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }

    public class UpdateUser
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}