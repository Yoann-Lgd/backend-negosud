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
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateBasket([FromBody] PanierInputDto panierInputDto)
    {
        var result = await _panierService.CreatePanier(panierInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateBasket([FromBody] PanierUpdateInputDto panierInputDto)
    {
        var result = await _panierService.UpdatePanier(panierInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    
}