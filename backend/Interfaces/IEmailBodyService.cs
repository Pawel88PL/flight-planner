namespace backend.Interfaces
{
    public interface IEmailBodyService
    {
        string ActivationEmailBody(string name, string link);
        string TwoFactorCodeEmailBody(string name, string twoFactorCode);
    }
}