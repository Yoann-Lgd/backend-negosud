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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ClientInputDto clientInputDto)
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
    public async Task<ActionResult> GetStockById(int id)
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