using AutoMapper;
using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class PanierService : IPanierService
{
    private ICommandeRepository _commandeRepository;
    private IClientRepository _clientRepository;
    private IArticleRepository _articleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PanierService> _logger;

    public PanierService(IMapper mapper, ICommandeRepository commandeRepository, IClientRepository clientRepository,
        IArticleRepository articleRepository, ILogger<PanierService> logger)
    {
        _commandeRepository = commandeRepository;
        _clientRepository = clientRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
        _logger = logger;
    }

public async Task<IResponseDataModel<PanierOutputDto>> CreatePanier(PanierInputDto panierInputDto)
{
    try
    {
        if (panierInputDto.ClientId <= 0)
        {
            _logger.LogError("ClientId est invalide.");
            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "ClientId invalide."
            };
        }

        _logger.LogInformation("Tentative de récupération du client avec ID: {ClientId}", panierInputDto.ClientId);
        var client = await _clientRepository.GetByIdAsync(panierInputDto.ClientId);

        if (client == null)
        {
            _logger.LogError("Client introuvable pour l'ID: {ClientId}", panierInputDto.ClientId);
            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Client introuvable.",
                StatusCode = 404
            };
        }

        // vérification si un panier actif existe déjà pour le client
        _logger.LogInformation("Vérification de l'existence d'un panier actif pour le client avec ID: {ClientId}", panierInputDto.ClientId);
        var existingPanier = await _commandeRepository.GetActiveBasketByClientIdAsync(panierInputDto.ClientId);

        Console.WriteLine(existingPanier);

        if (existingPanier != null)
        {
            _logger.LogInformation("Panier actif trouvé pour le client avec ID: {ClientId}. ID du panier: {PanierId}", panierInputDto.ClientId, existingPanier.CommandeId);
            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Un panier actif existe déjà pour ce client.",
                StatusCode = 409 
            };
        }

        _logger.LogInformation("Aucun panier actif trouvé pour le client avec ID: {ClientId}. Création d'un nouveau panier.", panierInputDto.ClientId);

        var ligneCommandes = _mapper.Map<List<LigneCommande>>(panierInputDto.LigneCommandes);
        foreach (var ligne in ligneCommandes)
        {
            var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == ligne.ArticleId);
            if (!articleExist)
            {
                _logger.LogWarning("Article ID {ArticleId} n'existe pas.", ligne.ArticleId);
            }
        }

        var commande = _mapper.Map<Commande>(panierInputDto);
        commande.LigneCommandes = ligneCommandes;
        commande.ExpirationDate = DateTime.UtcNow.AddHours(1);

        await _commandeRepository.AddAsync(commande);

        var createdCommande = await _commandeRepository.GetByIdAndLigneCommandesAsync(commande.CommandeId);

        var outputDto = _mapper.Map<PanierOutputDto>(createdCommande);

        return new ResponseDataModel<PanierOutputDto>
        {
            Success = true,
            Message = "Panier créé avec succès.",
            StatusCode = 201,
            Data = outputDto
        };
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Erreur lors de la création du panier");

        return new ResponseDataModel<PanierOutputDto>
        {
            Success = false,
            Message = "Une erreur s'est produite lors de la création du panier.",
            StatusCode = 500,
            Data = null,
        };
    }
}




    public async Task<IResponseDataModel<PanierOutputDto>> UpdatePanier(PanierUpdateInputDto panierUpdateDto)
    {
        try
        {
            if (panierUpdateDto.CommandId <= 0 || panierUpdateDto.ArticleId <= 0)
            {
                _logger.LogError("CommandId ou ArticleId invalide.");
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "CommandId ou ArticleId invalide."
                };
            }

            _logger.LogInformation("Tentative de récupération du panier avec CommandId: {CommandId}", panierUpdateDto.CommandId);
            var panier = await _commandeRepository.GetByIdAndLigneCommandesAsync(panierUpdateDto.CommandId);

            if (panier == null)
            {
                _logger.LogError("Panier introuvable pour l'ID: {CommandId}", panierUpdateDto.CommandId);
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "Panier introuvable.",
                    StatusCode = 404
                };
            }

            var existingLigne = panier.LigneCommandes.FirstOrDefault(lc => lc.ArticleId == panierUpdateDto.ArticleId);
            if (existingLigne != null)
            {
                if (panierUpdateDto.NewQuantite == 0)
                {
                    // suppression de la ligne de commande si la nouvelle quantité est de 0
                    panier.LigneCommandes.Remove(existingLigne);
                    _logger.LogInformation("Ligne de commande supprimée pour l'article ID: {ArticleId}", panierUpdateDto.ArticleId);
                }
                else
                {
                    existingLigne.Quantite = panierUpdateDto.NewQuantite;
                }
            }
            else
            {
                var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == panierUpdateDto.ArticleId);
                if (articleExist)
                {
                    var newLigneCommande = new LigneCommande
                    {
                        ArticleId = panierUpdateDto.ArticleId,
                        Quantite = panierUpdateDto.NewQuantite,
                        CommandeId = panierUpdateDto.CommandId
                    };
                    panier.LigneCommandes.Add(newLigneCommande);
                }
                else
                {
                    _logger.LogWarning("Article ID {ArticleId} n'existe pas.", panierUpdateDto.ArticleId);
                }
            }

            panier.ExpirationDate = DateTime.UtcNow.AddHours(1);
            await _commandeRepository.UpdateAsync(panier);

            // charger les articles associés aux lignes de commande
            foreach (var ligneCommande in panier.LigneCommandes)
            {
                ligneCommande.Article = await _articleRepository.GetByIdAsync(ligneCommande.ArticleId);
            }

            var outputDto = _mapper.Map<PanierOutputDto>(panier);

            return new ResponseDataModel<PanierOutputDto>
            {
                Success = true,
                Message = "Panier mis à jour avec succès.",
                StatusCode = 200,
                Data = outputDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la mise à jour du panier.");

            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la mise à jour du panier.",
                StatusCode = 500,
                Data = null,
            };
        }
    }





    public async Task<IResponseDataModel<string>> DeletePanier(int id)
    {
        try
        {
            var panier = await _commandeRepository.GetByIdAsync(id);
            if (panier == null)
            {
                _logger.LogError("Panier introuvable pour l'ID: {PanierId}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Panier introuvable.",
                    StatusCode = 404
                };
            }

            if (panier.Valide)
            {
                _logger.LogError("Le panier est déjà validé.");
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le panier est déjà validé.",
                    StatusCode = 400
                };
            }


            await _commandeRepository.DeleteAsync(panier);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Panier supprimé avec succès.",
                StatusCode = 200,
                Data = id.ToString()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression du panier.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression du panier.",
                StatusCode = 500,
                Data = id.ToString()
            };
        }
    }

    public async Task<IResponseDataModel<PanierOutputDto>> GetBasketByClientId(int id)
    {
        try
        {
            var commande = await _commandeRepository.GetActiveBasketByClientIdAsync(id);

            if (commande == null)
            {
                _logger.LogWarning("Aucun panier actif trouvé pour le client avec l'ID: {ClientId}", id);
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "Aucun panier actif trouvé pour ce client.",
                    StatusCode = 200,
                    Data = null
                };
            }

            var outputDto = _mapper.Map<PanierOutputDto>(commande);

            return new ResponseDataModel<PanierOutputDto>
            {
                Success = true,
                Data = outputDto,
                Message = "Panier bien récupéré",
                StatusCode = 200,
            };
        }
        catch (AutoMapperMappingException mappingException)
        {
            _logger.LogError(mappingException, "Erreur de mappage lors de la récupération du panier.");
            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Une erreur de mappage s'est produite.",
                StatusCode = 500,
                Data = null
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération du panier du client.");
            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération du panier.",
                StatusCode = 500,
                Data = null
            };
        }
    }


    public async Task<IResponseDataModel<string>> ExtendDurationBasket(int id)
    {
        try
        {
            var panier = await _commandeRepository.GetActiveBasketByClientIdAsync(id);

            if (panier == null)
            {
                _logger.LogWarning("Aucun panier actif trouvé pour le client avec l'ID: {ClientId}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Aucun panier actif trouvé pour ce client.",
                    StatusCode = 404,
                    Data = null
                };
            }

            panier.ExpirationDate = DateTime.UtcNow.AddHours(1);
            await _commandeRepository.UpdateAsync(panier);

            return new ResponseDataModel<string>
            {
                Success = true,
                Data = panier.CommandeId.ToString(),
                Message = "Panier prolongé avec succés",
                StatusCode = 200,
            };
        }
        catch (AutoMapperMappingException mappingException)
        {
            _logger.LogError(mappingException, "Erreur de mappage lors de la récupération du panier.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur de mappage s'est produite.",
                StatusCode = 500,
                Data = null
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération du panier du client.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération du panier.",
                StatusCode = 500,
                Data = null
            };
        }
    }

   public async Task<IResponseDataModel<string>> DeleteLigneCommande(PanierDeleteLigneInputDto panierDeleteLigneInput)
    {
        try
        {
            // vérification si le panier existe
            var panier = await _commandeRepository.GetByIdAndLigneCommandesAsync(panierDeleteLigneInput.CommandId);
            if (panier == null)
            {
                _logger.LogError("Panier introuvable pour l'ID: {PanierId}", panierDeleteLigneInput.CommandId);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Panier introuvable.",
                    StatusCode = 404
                };
            }

            // si le panier est déjà validé
            if (panier.Valide)
            {
                _logger.LogError("Le panier est déjà validé.");
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le panier est déjà validé.",
                    StatusCode = 400
                };
            }

            // on trouve la ligne de commande à supprimer
            var ligneCommande = panier.LigneCommandes.FirstOrDefault(lc => lc.LigneCommandeId == panierDeleteLigneInput.LigneCommandeId);
            if (ligneCommande == null)
            {
                _logger.LogError("Ligne de commande introuvable pour l'ID: {LigneCommandeId}", panierDeleteLigneInput.LigneCommandeId);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Ligne de commande introuvable.",
                    StatusCode = 404
                };
            }

            // on la supprime
            panier.LigneCommandes.Remove(ligneCommande);

            // on met à jour les modifications
            await _commandeRepository.UpdateAsync(panier);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Ligne de commande supprimée avec succès.",
                StatusCode = 200
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression d'une ligne de commande.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression d'une ligne de commande",
                StatusCode = 500,
            };
        }
    }



    public async Task<IResponseDataModel<CommandeOutputDto>> BasketToCommand(int id)
    {
        try
        {
            var commande = await _commandeRepository.GetActiveBasketByClientIdAsync(id);

            if (commande == null)
            {
                _logger.LogWarning("Aucun panier actif trouvé pour le client avec l'ID: {ClientId}", id);
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    Message = "Aucun panier actif trouvé pour ce client.",
                    StatusCode = 404,
                    Data = null
                };
            }

            commande.Valide = true;
            commande.DateCreation = DateTime.UtcNow;
            commande.ExpirationDate = null; 
            
            var livraison = new Livraison
            {
                DateEstimee = DateTime.UtcNow.AddDays(5), //date purement arbitraire, 5 jours 
                Livree = false
            };

            commande.Livraison = livraison;

            await _commandeRepository.UpdateAsync(commande);
            
            var outputDto = _mapper.Map<CommandeOutputDto>(commande);

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = true,
                Data = outputDto,
                Message = "La commande a bien été créée",
                StatusCode = 200,
            };
        }
        catch (AutoMapperMappingException mappingException)
        {
            _logger.LogError(mappingException, "Erreur de mappage lors de la création de la commande");
            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur de mappage s'est produite.",
                StatusCode = 500,
                Data = null
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la crátion de la commande.");
            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la crátion de la commande.",
                StatusCode = 500,
                Data = null
            };
        }
    }
}