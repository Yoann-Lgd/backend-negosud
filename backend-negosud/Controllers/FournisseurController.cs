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
}