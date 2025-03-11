using backend_negosud.DTOs.Commande_fournisseur.Inputs;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BonCommandeController : ControllerBase
{
    private readonly IBonCommandeService _bonCommandeService;
    
    public BonCommandeController(IBonCommandeService bonCommandeService)
    {
        _bonCommandeService = bonCommandeService;
    }
    
    
    // POST: api/boncommande
    /// <summary>
    /// Création d'un bon de commande. 
    /// </summary>
    /// <returns>Un booléan response</returns>
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateCommande([FromBody] BonCommandeCreateInputDto bonCommandeCreateInput)
    {
        var result = await _bonCommandeService.CreateBonCommande(bonCommandeCreateInput);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }

}