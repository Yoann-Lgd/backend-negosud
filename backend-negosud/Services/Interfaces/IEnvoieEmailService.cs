namespace backend_negosud.Services;

public interface IEnvoieEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
}