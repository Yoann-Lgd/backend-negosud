namespace backend_negosud.Services;

public interface IVerificationEmailService
{
    Task<bool> EmailExistsAsync(string email);
}