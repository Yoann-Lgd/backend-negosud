using backend_negosud.DTOs.Fournisseur.FournisseurInputDto;
using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FournisseurController : ControllerBase
{
    private IFournisseurService _fournisseurService;

    public FournisseurController(IFournisseurService fournisseurService)
    {
        _fournisseurService = fournisseurService;
    }

    /// <summary>
    /// Récupère tous les fournisseurs, inclut les articles et les adresses du fournisseur
    /// </summary>
    /// <returns>List<FournisseurCompleteDTO></returns>
    [HttpGet]
    public async Task<IActionResult> getAllFournisseur()
    {
        var result = await _fournisseurService.getAll();
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    
    /// <summary>
    /// Crée un nouveau fournisseur
    /// </summary>
    /// <returns> Un code 200</returns>
    [HttpPost]
    public async Task<IActionResult> CreateFournisseur(FournisseurInputMinimal fournisseurInputMinimal)
    {
        var result = await _fournisseurService.CreateFournisseur(fournisseurInputMinimal);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    // DELETE: api/commande
    /// <summary>
    ///  SoftDelete du fournisseur, il faut renseigner l'id du fournisseur. 
    /// </summary>
    /// <returns>Retourne l'id du fournisseur qui a été softdelete</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteCommande(int id)
    {
        var result = await _fournisseurService.softDeleteFournisseurById(id);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    // PATCH: api/fournisseur
    /// <summary>
    ///  Modification du fournisseur, modification d'un champ ou de plusieurs
    /// </summary>
    /// <returns>Retourne l'id du fournisseur qui a été modifié</returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchFournisseur(int id,[FromBody] FournisseurInputMinimal fournisseurInputMinimal)
    {
        var result = await _fournisseurService.PatchMinimalFournisseur(id, fournisseurInputMinimal);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
}