using AutoMapper;
using backend_negosud.DTOs.Article.ArticleInputDto;
using backend_negosud.DTOs.Article.ArticleOutputDto;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;
using FluentValidation.Results;

namespace backend_negosud.Services;

public class ArticleService : IArticleService
{
    private IArticleRepository _articleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(IArticleRepository articleRepository, IMapper mapper, ILogger<ArticleService> logger)
    {
        _articleRepository = articleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResponseDataModel<ArticleEssentialOutputDto>> getArticleById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour récupérer l'article : {Id}", id);
                return new ResponseDataModel<ArticleEssentialOutputDto>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }

            var article = await _articleRepository.GetCompletArticleById(id);
            var output = _mapper.Map<ArticleEssentialOutputDto>(article);
            if (article != null)
            {
                return new ResponseDataModel<ArticleEssentialOutputDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Data = output,
                };
            }
            else
            {
                _logger.LogWarning("Aucun article trouvé avec l'identifiant : {Id}", id);
                return new ResponseDataModel<ArticleEssentialOutputDto>
                {
                    Success = false,
                    Message = "Aucun article trouvé avec cet identifiant.",
                    StatusCode = 404,
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la récupération de l'article avec l'identifiant : {Id}", id);
            return new ResponseDataModel<ArticleEssentialOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération de l'article.",
                StatusCode = 500,
            };
        }
    }
    

    public async Task<IResponseDataModel<List<ArticleEssentialOutputDto>>> getAll()
    {
        try
        {
            var articles = await _articleRepository.GetAllCompletArticlesAsync();
            var output = _mapper.Map<List<ArticleEssentialOutputDto>>(articles);

            return new ResponseDataModel<List<ArticleEssentialOutputDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = output,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la récupération des articles.");
            return new ResponseDataModel<List<ArticleEssentialOutputDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des articles.",
                StatusCode = 500,
            };
        }
    }

    public async Task<IResponseDataModel<string>> CreateArticle(ArticleInputCreateDto articleInput)
    {
        try
        {
            // Valider les données d'entrée
            var validator = new ArticleInputCreateDtoValidator();
            var validationResult = validator.Validate(articleInput);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                return new ResponseDataModel<string>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors),
                };
            }

            // Mapper les données d'entrée vers l'entité
            var article = _mapper.Map<Article>(articleInput);

            // Enregistrer l'entité dans la base de données
            await _articleRepository.AddAsync(article);

            _logger.LogInformation("L'article avec l'ID {ArticleId} a été créé avec succès.", article.ArticleId);

            return new ResponseDataModel<string>
            {
                Success = true,
                StatusCode = 201,
                Message = "L'article a été créé avec succès.",
                Data = article.ArticleId.ToString(),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Un problème est survenu pendant la création de l'article.");
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 500,
                Message = "Un problème est survenu pendant la création de l'article.",
            };
        }
    }
    
    public async Task<IResponseDataModel<string>> UpdateArticle(ArticleUpdateInputDto articleInput)
    {
        // vérif des données entrée vuia fluent
        var validator = new ArticleUpdateValidator();
        ValidationResult validationResult = validator.Validate(articleInput);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 400,
                Message = string.Join(", ", errors),
            };
        }

        try
        {
            var article = await _articleRepository.GetByIdAsync(articleInput.ArticleId);
            if (article == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Article non trouvé.",
                };
            }
            
            if (!string.IsNullOrEmpty(articleInput.Libelle))
            {
                article.Libelle = articleInput.Libelle;
            }

            if (!string.IsNullOrEmpty(articleInput.Reference))
            {
                article.Reference = articleInput.Reference;
            }

            if (articleInput.Prix > 0)
            {
                article.Prix = articleInput.Prix;
            }

            if (articleInput.FamilleId != 0)
            {
                article.FamilleId = articleInput.FamilleId;
            }

            if (articleInput.FournisseurId != 0)
            {
                article.FournisseurId = articleInput.FournisseurId;
            }

            if (articleInput.TvaId != 0)
            {
                article.TvaId = articleInput.TvaId;
            }

            await _articleRepository.UpdateAsync(article);

            return new ResponseDataModel<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "L'article a été mis à jour avec succès.",
                Data = article.ArticleId.ToString(),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la mise à jour de l'article.");
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 500,
                Message = "Une erreur s'est produite lors de la mise à jour de l'article.",
            };
        }
    }

    public async Task<IResponseDataModel<string>> DeleteArticleById(int id)
    {
        try
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
            {
                _logger.LogError("article introuvable pour l'ID: {articleId}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "article introuvable.",
                    StatusCode = 404
                };
            }

            await _articleRepository.DeleteAsync(article);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "article supprimé avec succès.",
                StatusCode = 200,
                Data = id.ToString()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression du article.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression du article.",
                StatusCode = 500,
                Data = id.ToString()
            };
        }
    }
    
    public async Task<IResponseDataModel<string>> PatchArticle(int id, ArticleUpdateInputDto articleInput)
    {
        // Créer une instance du validateur
        var validator = new ArticleUpdateValidator();
        ValidationResult validationResult = validator.Validate(articleInput);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 400,
                Message = string.Join(", ", errors),
            };
        }

        try
        {
            var article = await _articleRepository.GetByIdAsync(articleInput.ArticleId);
            if (article == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Article non trouvé.",
                    Data = article.ArticleId.ToString(),
                };
            }

            // mis à jour uniquement des champs fournis
            if (!string.IsNullOrEmpty(articleInput.Libelle))
            {
                article.Libelle = articleInput.Libelle;
            }

            if (!string.IsNullOrEmpty(articleInput.Reference))
            {
                article.Reference = articleInput.Reference;
            }

            if (articleInput.Prix > 0)
            {
                article.Prix = articleInput.Prix;
            }

            if (articleInput.FamilleId != 0)
            {
                article.FamilleId = articleInput.FamilleId;
            }

            if (articleInput.FournisseurId != 0)
            {
                article.FournisseurId = articleInput.FournisseurId;
            }

            if (articleInput.TvaId != 0)
            {
                article.TvaId = articleInput.TvaId;
            }

            await _articleRepository.UpdateAsync(article);

            return new ResponseDataModel<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "L'article a été mis à jour avec succès.",
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la mise à jour de l'article.");
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 500,
                Message = "Une erreur s'est produite lors de la mise à jour de l'article.",
            };
        }
    }
}