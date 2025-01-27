using backend_negosud.Entities;

namespace backend_negosud.Services;

public interface IJwtService<TEntity, TInputDto, TOutputDto>
    where TEntity : class
    where TInputDto : class
    where TOutputDto : class
{
    string GenererToken(TInputDto inputDto);
    Task<TOutputDto> ValidateToken(string token, int id);
}