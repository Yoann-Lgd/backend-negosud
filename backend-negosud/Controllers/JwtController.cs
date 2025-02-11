using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtController<TEntity, TInputDto, TOutputDto> : ControllerBase
    where TEntity : class
    where TInputDto : class
    where TOutputDto : class
{
    private readonly IJwtService<TEntity, TInputDto, TOutputDto> _jwtService;

    public JwtController(JwtService<TEntity, TInputDto, TOutputDto> jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpGet("getPublicKey")]
    public async Task<IActionResult> getPublicKey()
    {
        var result = await _jwtService.GetPublicKey();
        return result != null ? Ok(result) : BadRequest("Failed to retrieve public key");
    }
}