namespace backend.Interfaces
{
    public interface IEmailService
    {
        Task SendActivationEmail(string userId, string token);
        Task SendTwoFactorCodeEmail(string userId, string twoFactorCode);
    }
}