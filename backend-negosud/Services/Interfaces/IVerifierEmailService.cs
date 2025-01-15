using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IVerifierEmailService
{
    Task<ResponseModel> Verifier(Guid token);
}