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
    
    
    // POST: api/BonCommande
    /// <summary>
    /// Création d'un bon de commande. 
    /// </summary>
    /// <returns>Un booléan response</returns>
    [HttpPost("create")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateCommande([FromBody] BonCommandeCreateInputDto bonCommandeCreateInput)
    {
        var result = await _bonCommandeService.CreateBonCommande(bonCommandeCreateInput);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    // GET: api/BonCommande
    /// <summary>
    /// </summary>
    /// <returns>Retourne toutes les commandes fournisseurs, les lignes de commandes</returns>
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> GetBonCommandes()
    {
        var result = await _bonCommandeService.GetAllBonCommandes();
        return result.Success ? Ok(result) : BadRequest(result);
    }      
    
    
    // GET: api/BonCommande/id
    /// <summary>
    /// </summary>
    /// <returns>Retourne la commande, les lignes de commandes</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> GetBonCommandeById(int id)
    {
        var result = await _bonCommandeService.GetBonCommandeById(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }        

    // PUT: api/BonCommande/{id}
    /// <summary>
    /// 
    /// </summary>
    /// <returns>La commande modifiée.</returns>
    [HttpPut("update/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateBasket(int id,[FromBody] BonCommandeUpdateDto bonCommandeUpdate)
    {
        var result = await _bonCommandeService.UpdateBonCommande(id,bonCommandeUpdate);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // DELETE: api/BonCommande/ligne-de-commande/{id}
    /// <summary>
    /// Endpoint qui sert à supprimmer une ligne de commande en passant dans le body : commandId, clientId et ligneCommandeId
    /// </summary>
    /// <returns>l'id de la ligne supprimée</returns>
    [HttpDelete("delete/ligne-commande/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteLigneDeCommande(int id)
    {
        var result = await _bonCommandeService.DeleteLigneCommande(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}