using backend_negosud.DTOs.Article.ArticleInputDto;
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
    
    // POST: api/article
    /// <summary>
    ///  Création d'un article en renseignant l'id d'une famille, d'un fournisseur et de la tva
    /// </summary>
    /// <returns>Retourne l'id de l'article qui a été créé</returns>
    [HttpPost("create")]
    public async Task<ActionResult> CreateArticle([FromBody] ArticleInputCreateDto articleInputCreateDto)
    {
        var result = await _articleService.CreateArticle(articleInputCreateDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }    
    
    // PUT: api/article
    /// <summary>
    ///  Modification de l'article
    /// </summary>
    /// <returns>Retourne l'id de l'article qui a été modifié</returns>
    [HttpPut("update")]
    public async Task<ActionResult> UpdateArticle([FromBody] ArticleUpdateInputDto articleUpdateInput)
    {
        var result = await _articleService.UpdateArticle(articleUpdateInput);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // PATCH: api/article
    /// <summary>
    ///  Modification de l'article, modification d'un champ ou de plusieurs
    /// </summary>
    /// <returns>Retourne l'id de l'article qui a été modifié</returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchArticle(int id, [FromBody] ArticleUpdateInputDto articleInput)
    {
        var result = await _articleService.PatchArticle(id, articleInput);
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }

    
}