using backend_negosud.DTOs;
using backend_negosud.DTOs.Utilisateur.Input;
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
    public async Task<IActionResult> Login([FromBody] utilisateurSimplifiedDto utilisateurInputDto)
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
    
    // POST: api/utilisateur
    /// <summary>
    /// Il faut renseigner l'adresse email. 
    /// </summary>
    /// <returns>Un booleen response. </returns>
    [HttpPost("exist")]
    public async Task<IActionResult> FindByEmail([FromBody] UtilisateurEmailInputDto emailInput)
    {
        var result = await _utilisateurService.UtilisateurExistEmail(emailInput);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // DELETE: api/utilisateur
    /// <summary>
    ///  SoftDelete de l'utilisateur, il faut renseigner l'id de l'utilisateur. 
    /// </summary>
    /// <returns>Retourne l'id de l'utilisateur qui a été softdelete</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteUtilisateur(int id)
    {
        var result = await _utilisateurService.SoftDeleteAsync(id);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUtilisateurs()
    {
        var result = await _utilisateurService.GetAllUtilisateurs();
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }

}