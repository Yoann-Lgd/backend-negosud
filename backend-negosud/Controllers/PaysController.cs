using backend_negosud.DTOs.Pays.PaysInputDto;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

public class PaysController : ControllerBase
{
    private IPaysService _paysService;

    public PaysController(IPaysService payssService)
    {
        _paysService = payssService;
    }
    
    // POST: api/pays
    /// <summary>
    /// Création d'un pays en lui passant un nom
    /// </summary>
    /// <returns>L'id du pays créé</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreatePays([FromBody] PaysInputDto panierInputDto)
    {
        var result = await _paysService.CreatePays(panierInputDto);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
    
    // GET: api/article
    /// <summary>
    /// Récupération du pays par rapport à l'id passé en param
    /// </summary>
    /// <returns>Retourne le pays en fonction de l'id passé</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetPays(int id)
    {
        var result = await _paysService.GetById(id);
        return result.Success ? Ok(result) :StatusCode(result.StatusCode, result);
    }    

    // GET: api/article
    /// <summary>
    /// Récupération de tous les pays
    /// </summary>
    /// <returns>List pays</returns>
    [HttpGet("pays")]
    public async Task<ActionResult> GetAllPays()
    {
        var result = await _paysService.GetAllPays();
        return result.Success ? Ok(result) :StatusCode(result.StatusCode, result);
    }
    
    // Delete: api/article
    /// <summary>
    /// Suppression d'un pays en lui passant un id
    /// </summary>
    /// <returns>Id du pays supp</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePays(int id)
    {
        var result = await _paysService.DeletePays(id);
        return result.Success ? Ok(result) :StatusCode(result.StatusCode, result);
    }
}