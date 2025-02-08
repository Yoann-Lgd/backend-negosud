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
            _logger.LogError(e, "Erreur lors de la récupération des commandes");

            return new ResponseDataModel<PanierOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500,
                Data = null,
            };
        }
    }

    public async Task<IResponseDataModel<PanierOutputDto>> UpdatePanier(PanierUpdateInputDto panierUpdateDto)
    {
        try
        {
            if (panierUpdateDto.ClientId <= 0)
            {
                _logger.LogError("ClientId est invalide.");
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "ClientId invalide."
                };
            }

            _logger.LogInformation("Tentative de récupération du client avec ID: {ClientId}", panierUpdateDto.ClientId);
            var client = await _clientRepository.GetByIdAsync(panierUpdateDto.ClientId);

            if (client == null)
            {
                _logger.LogError("Client introuvable pour l'ID: {ClientId}", panierUpdateDto.ClientId);
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "Client introuvable.",
                    StatusCode = 404
                };
            }

            var panier = await _commandeRepository.GetByIdAndLigneCommandesAsync(panierUpdateDto.CommandId);
            if (panier == null)
            {
                _logger.LogError("Panier introuvable pour l'ID: {PanierId}", panierUpdateDto.CommandId);
                return new ResponseDataModel<PanierOutputDto>
                {
                    Success = false,
                    Message = "Panier introuvable.",
                    StatusCode = 404
                };
            }

            var existingLigneCommandes = panier.LigneCommandes.ToList();
            foreach (var newLigneDto in panierUpdateDto.LigneCommandes)
            {
                var existingLigne =
                    existingLigneCommandes.FirstOrDefault(lc => lc.LigneCommandeId == newLigneDto.LigneCommandeId);
                if (existingLigne != null)
                {
                    existingLigne.Quantite = newLigneDto.Quantite;
                }
                else
                {
                    var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == newLigneDto.ArticleId);
                    if (articleExist)
                    {
                        var newLigneCommande = _mapper.Map<LigneCommande>(newLigneDto);
                        panier.LigneCommandes.Add(newLigneCommande);
                    }
                    else
                    {
                        _logger.LogWarning("Article ID {ArticleId} n'existe pas.", newLigneDto.ArticleId);
                    }
                }
            }

            // supprime les lignes de commande qui ne sont plus présentes dans le DTO
            var ligneCommandesToRemove = existingLigneCommandes.Where(lc =>
                !panierUpdateDto.LigneCommandes.Any(dto => dto.LigneCommandeId == lc.LigneCommandeId)).ToList();
            foreach (var ligneCommande in ligneCommandesToRemove)
            {
                panier.LigneCommandes.Remove(ligneCommande);
            }
            
            panier.ExpirationDate = DateTime.UtcNow.AddDays(7);
            await _commandeRepository.UpdateAsync(panier);

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
                    StatusCode = 404,
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
}