using backend_negosud.DTOs.Famille;
using backend_negosud.DTOs.Famille.Outputs;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FamilleController : ControllerBase
{
    private readonly IFamilleService _familleService;

    public FamilleController(IFamilleService familleService)
    {
        _familleService = familleService;
    }
    
    [HttpPost("create-famille")]
    public async Task<IActionResult> Register([FromBody] FamilleCreateInputDto familleCreateInputDto)
    {
        var result = await _familleService.CreateFamille(familleCreateInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<List<FamilleOutputDto>>>> GetAllStocks()
    {
        var familles = await _familleService.GetAllFamilles();
        return familles.Success ? Ok(familles) : BadRequest(familles);
    }    
    
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<FamilleOutputDto>>> getById(int id)
    {
        var famille = await _familleService.getFamilleById(id);
        return famille.Success ? Ok(famille) : BadRequest(famille);
    }
}