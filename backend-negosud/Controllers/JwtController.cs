using backend_negosud.DTOs;
using backend_negosud.Services;
using backend_negosud.DTOs.Utilisateur;
using backend_negosud.Entities;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtController : ControllerBase
{
    private readonly IJwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto> _jwtService;

    public JwtController(IJwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto> jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpGet("public-key")]
    public async Task<IActionResult> GetPublicKey()
    {
        var result = await _jwtService.GetPublicKey();
        return result != null ? Ok(result) : BadRequest("Failed to retrieve public key");
    }
}