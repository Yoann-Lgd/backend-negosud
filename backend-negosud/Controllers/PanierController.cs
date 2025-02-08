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
    
    /// <summary>
    /// Il faut fournir un au minimum : un id d'un client
    /// </summary>
    /// <returns>Le panier du client</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBasketClient(int id)
    {
        var result = await _panierService.GetBasketByClientId(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // POST: api/panier
    /// <summary>
    /// Il faut fournir un au minimum : un id d'un client, un id d'un article, une quantité 
    /// </summary>
    /// <returns>Le panier créé</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateBasket([FromBody] PanierInputDto panierInputDto)
    {
        var result = await _panierService.CreatePanier(panierInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT: api/panier
    /// <summary>
    /// Il faut fournir un au minimum : l'id commande, l'articleId. Si, une ligne de commande existe déjà il faudra préciser son Id sinon une autre ligne va être créée
    /// </summary>
    /// <returns>Le panier modifié</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateBasket([FromBody] PanierUpdateInputDto panierInputDto)
    {
        var result = await _panierService.UpdatePanier(panierInputDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // PUT: api/panier
    /// <summary>
    /// Il faut fournir un au minimum : l'id du client.
    /// </summary>
    /// <returns>Le panier modifié</returns>
    [HttpPut("extend/{id}")]
    public async Task<IActionResult> ExtendBasketDuration(int id)
    {
        var result = await _panierService.ExtendDurationBasket(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }    
    
    // PUT: api/panier/valid/{od}
    /// <summary>
    /// Il faut fournir un au minimum : l'id du client.
    /// </summary>
    /// <returns>La commande</returns>
    [HttpPut("valid/{id}")]
    public async Task<IActionResult> BasketToCommand(int id)
    {
        var result = await _panierService.BasketToCommand(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    
    // DELETE: api/panier/{id}
    /// <summary>
    /// L'entité panier n'existe que si l'attribut valide est à false. Une vérification est faite sur ce critère pour supprimer le panier (S'il en est un dans le sens de notre logique métier)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>l'id du panier supprimer</returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteBasket(int id)
    {
        var result = await _panierService.DeletePanier(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    
}