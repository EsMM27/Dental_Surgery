namespace Dental_Surgery.DataAccess.Repo
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
