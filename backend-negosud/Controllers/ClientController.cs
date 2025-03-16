using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientInputDto clientDto)
    {
        var result = await _clientService.CreateClient(clientDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("validate-email")]
    public async Task<IActionResult> ValidateEmail([FromBody] EmailValidationDto validationDto)
    {
        var result = await _clientService.VerifierCodeValidation(validationDto.Email, validationDto.Code);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // POST: api/client
    /// <summary>
    /// Il faut renseigner l'adresse email du client 
    /// </summary>
    /// <returns>Un booleen response. </returns>
    [HttpPost("exist")]
    public async Task<IActionResult> FindByEmail([FromBody] ClientEmailInputDto emailInput)
    {
        var result = await _clientService.ClientExistEmail(emailInput);
        return result.Success ? Ok(result) : BadRequest(result);
    }    
    
    // DELETE: api/client
    /// <summary>
    ///  SoftDelete du client, il faut renseigner juste l'id du client. 
    /// </summary>
    /// <returns>Retourne l'id de l'article qui a été softdelete</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteClient(int id)
    {
        var result = await _clientService.SoftDeleteClientAsync(id);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ClientInputDtoSimplified clientInputDto)
    {
        var result = await _clientService.Login(clientInputDto.Email, clientInputDto.MotDePasse);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ClientInputDto clientInputDto)
    {
        var result = await _clientService.ResetMotDePasse(clientInputDto.Email);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("protected-test")]
    [Authorize]
    public IActionResult ProtectedTest()
    {
        return Ok("Hello authorized client");
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetClientById(int id)
    {
        var result = await _clientService.GetClientBydId(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("resend-validation")]
    public async Task<IActionResult> ResendValidation([FromBody] ClientInputDto clientInputDto)
    {
        var result = await _clientService.ValidationClientEmail(clientInputDto);
        return result ? Ok(new { Success = true, Message = "Code de validation renvoyé" }) 
                     : BadRequest(new { Success = false, Message = "Échec de l'envoi du code de validation" });
    }
}