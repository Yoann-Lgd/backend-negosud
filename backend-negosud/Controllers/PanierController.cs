using backend_negosud.DTOs.Commande_client;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PanierController : ControllerBase
{
    private readonly IPanierService _panierService;

    public PanierController(IPanierService panierService)
    {
        _panierService = panierService;
    }
    
    [HttpPost("create-basket")]
    public async Task<IActionResult> Register([FromBody] PanierCreateInputDto panierCreateInputDto)
    {
        var result = await _panierService.CreatePanier(panierCreateInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    
    
}