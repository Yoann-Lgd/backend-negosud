using backend_negosud.Entities;

namespace backend_negosud.Repository;

public interface IArticleRepository : IRepositoryBase<Article>
{
    Task<Article> GetCompletArticleById(int id);
    Task<List<Article>> GetAllCompletArticlesAsync();
    Task<List<Article>> GetArticlesByFournisseurAsync(int fournisseurId);
}