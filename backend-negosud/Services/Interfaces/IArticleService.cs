using backend_negosud.DTOs.Article.ArticleInputDto;
using backend_negosud.DTOs.Article.ArticleOutputDto;

using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IArticleService
{
    Task<IResponseDataModel<ArticleEssentialOutputDto>> getArticleById(int id);
    Task<IResponseDataModel<List<ArticleEssentialOutputDto>>> getAll();
    Task<IResponseDataModel<string>> CreateArticle(ArticleInputCreateDto articleInput);
    Task<IResponseDataModel<string>> UpdateArticle(ArticleUpdateInputDto articleInput);
    Task<IResponseDataModel<string>> PatchArticle(int id, ArticleUpdateInputDto articleInput);

}