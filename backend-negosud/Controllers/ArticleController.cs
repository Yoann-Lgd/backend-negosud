using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    // GET: api/article/{id}
    /// <summary>
    /// Fournir l'id de l'article
    /// </summary>
    /// <returns>L'article complet ainsi que le détail de ses relations (Fournisseur, Tva, Famille)</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetArticleById(int id)
    {
        var result = await _articleService.getArticleById(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }    
    
    // GET: api/article
    /// <summary>
    /// </summary>
    /// <returns>Retourne tous les articles ainsi que leurs relations (Fournisseur, Tva, Famille)</returns>
    [HttpGet]
    public async Task<ActionResult> GetArticles()
    {
        var result = await _articleService.getAll();
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
}