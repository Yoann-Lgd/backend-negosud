using backend_negosud.DTOs.Commande_client;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandeController : ControllerBase
{
    private readonly ICommandeService _commandeService;

    public CommandeController(ICommandeService commandeService)
    {
        _commandeService = commandeService;
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _commandeService.GetAllCommandes());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommandeById(int id)
    {
        return Ok(await _commandeService.GetCommandeById(id));
    }
    
    // POST: api/commande
    /// <summary>
    /// Création d'une commande sans passer par la phase panier. 
    /// </summary>
    /// <returns>La commande créée</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateCommande([FromBody] CommandeInputDto commandeInput)
    {
        var result = await _commandeService.CreateCommande(commandeInput);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
}