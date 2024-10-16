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
    }

    public class UserDTO
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DateAdded { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool TermsAccepted { get; set; }
    }

    public class Login
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class Register
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
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

    public class TwoFactorRequest
    {
        public string? Id { get; set; } // Id użytkownika, który próbuje się zalogować
        public string? Code { get; set; } // Kod 2FA, który użytkownik otrzymał i wprowadził
    }
}