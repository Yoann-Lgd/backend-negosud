using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilisateurController : ControllerBase
{
    private readonly IUtilisateurService _utilisateurService;


    public UtilisateurController(
        IUtilisateurService utilisateurService)
    {
        _utilisateurService = utilisateurService;
        
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UtilisateurInputDto utilisateurDto)
    {
        var result = await _utilisateurService.CreateUtilisateur(utilisateurDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UtilisateurInputDto utilisateurInputDto)
    {
        var result = await _utilisateurService.Login(utilisateurInputDto.Email, utilisateurInputDto.MotDePasse);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("resetmotdepasse")]
    public async Task<IActionResult> ResetMotDePasse([FromBody] UtilisateurInputDto utilisateurInputDto)
    {
        var result = await _utilisateurService.ResetMotDePasse(utilisateurInputDto.Email);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("protectedTest")]
    [Authorize]
    public IActionResult ProtectedTest()
    {
        return Ok("Hello autorized");
    }
}