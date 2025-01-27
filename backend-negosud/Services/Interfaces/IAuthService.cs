using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IAuthService<TEntity, TInputDto, TOutputDto>
    where TEntity : class
    where TInputDto : class
    where TOutputDto : class
{
    Task<IResponseDataModel<TOutputDto>> Login(string email, string motDePasse);
    Task<IResponseDataModel<string>> ResetMotDePasse(string email);
}