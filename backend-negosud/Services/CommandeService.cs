using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Enums;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class CommandeService
{
    private readonly PostgresContext _context;
    private readonly ICommandeRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommandeService> _logger;

    public CommandeService(ICommandeRepository repository, PostgresContext context, IMapper mapper, ILogger<CommandeService> logger)
    {
        _repository = repository;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<IResponseDataModel<CommandeOutputDto>> GetPanierByClientId(int clientId)
    {
        try
        {
            var commande = await _repository.GetCommandeByStatut(clientId, StatutCommande.Panier);

            if (commande == null)
            {
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    Message = "Aucun panier trouvé",
                    StatusCode = 404
                };
            }

            var commandeDto = _mapper.Map<CommandeOutputDto>(commande);

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = true,
                Data = commandeDto,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du panier");
            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = false,
                Message = "Erreur interne du serveur",
                StatusCode = 500
            };
        }
    }

    // ajouter un Article au panier
    public async Task<IResponseDataModel<string>> AjouterArticleAuPanier(int clientId, int ArticleId, int quantite)
    {
        try
        {
            var commande = await _repository.GetCommandeByStatut(clientId, StatutCommande.Panier);

            if (commande == null)
            {
                commande = new Commande
                {
                    ClientId = clientId,
                    Statut = StatutCommande.Panier,
                    DateCreation = DateTime.Now,
                    LigneCommandes = new List<LigneCommande>()
                };

                await _repository.AddAsync(commande);
            }

            var articleExistant = commande.LigneCommandes.FirstOrDefault(lc => lc.Article.ArticleId == ArticleId);

            if (articleExistant != null)
            {
                articleExistant.Quantite += quantite;
            }
            else
            {
                commande.LigneCommandes.Add(new LigneCommande
                {
                    articleId = articleId,
                    Quantite = quantite,
                    PrixUnitaire = await _repository.GetPrixarticle(articleId)
                });
            }

            await _repository.UpdateAsync(commande);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "article ajouté au panier",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'ajout du article au panier");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Erreur interne du serveur",
                StatusCode = 500
            };
        }
    }

    // modifie la quantité d’un article
    public async Task<IResponseDataModel<string>> ModifierQuantitearticlePanier(int clientId, int articleId, int nouvelleQuantite)
    {
        try
        {
            var commande = await _repository.GetCommandeByStatut(clientId, StatutCommande.Panier);

            if (commande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Panier introuvable",
                    StatusCode = 404
                };
            }

            var ligneCommande = commande.LigneCommandes.FirstOrDefault(lc => lc.articleId == articleId);

            if (ligneCommande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "article non trouvé dans le panier",
                    StatusCode = 404
                };
            }

            if (nouvelleQuantite <= 0)
            {
                commande.LigneCommandes.Remove(ligneCommande);
            }
            else
            {
                ligneCommande.Quantite = nouvelleQuantite;
            }

            await _repository.UpdateAsync(commande);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Quantité mise à jour",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la modification de la quantité du article");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Erreur interne du serveur",
                StatusCode = 500
            };
        }
    }

    // supprimer un article du panier
    public async Task<IResponseDataModel<string>> SupprimerarticleDuPanier(int clientId, int articleId)
    {
        try
        {
            var commande = await _repository.GetCommandeByStatut(clientId, StatutCommande.Panier);

            if (commande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Panier introuvable",
                    StatusCode = 404
                };
            }

            var ligneCommande = commande.LigneCommandes.FirstOrDefault(lc => lc.articleId == articleId);

            if (ligneCommande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "article non trouvé dans le panier",
                    StatusCode = 404
                };
            }

            commande.LigneCommandes.Remove(ligneCommande);
            await _repository.UpdateAsync(commande);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "article supprimé du panier",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du article du panier");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Erreur interne du serveur",
                StatusCode = 500
            };
        }
    }

    // valider le panier (conversion en commande)
    public async Task<IResponseDataModel<string>> ValiderPanier(int clientId)
    {
        try
        {
            var commande = await _repository.GetCommandeByStatut(clientId, StatutCommande.Panier);

            if (commande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Aucun panier trouvé",
                    StatusCode = 404
                };
            }

            commande.Statut = StatutCommande.Validée;
            commande.DateValidation = DateTime.Now;

            await _repository.UpdateAsync(commande);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Commande validée avec succès",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du panier");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Erreur interne du serveur",
                StatusCode = 500
            };
        }
    }