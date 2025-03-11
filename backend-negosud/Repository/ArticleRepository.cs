using AutoMapper;
using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<ClientRepository> _logger;
    
    public ArticleRepository(
        PostgresContext context, 
        ILogger<ClientRepository> logger) : base(context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Article> GetCompletArticleById(int id)
    {
        return await _context.Articles
            .Include(a => a.Famille)
            .Include(a => a.Fournisseur)
            .Include(a => a.Tva)
            .FirstOrDefaultAsync(a => a.ArticleId == id);
    }
    
    public async Task<List<Article>> GetAllCompletArticlesAsync()
    {
        return await _context.Articles
            .Include(a => a.Famille)
            .Include(a => a.Fournisseur)
            .Include(a => a.Tva)
            .ToListAsync();
    }


}