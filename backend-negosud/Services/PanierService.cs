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

    public PanierService(IMapper mapper, ICommandeRepository commandeRepository,IClientRepository clientRepository, IArticleRepository articleRepository, ILogger<PanierService> logger)
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
            
            await _commandeRepository.AddAsync(commande);
            
            var outputDto = _mapper.Map<PanierOutputDto>(commande);

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
                var existingLigne = existingLigneCommandes.FirstOrDefault(lc => lc.LigneCommandeId == newLigneDto.LigneCommandeId);
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
            var ligneCommandesToRemove = existingLigneCommandes.Where(lc => !panierUpdateDto.LigneCommandes.Any(dto => dto.LigneCommandeId == lc.LigneCommandeId)).ToList();
            foreach (var ligneCommande in ligneCommandesToRemove)
            {
                panier.LigneCommandes.Remove(ligneCommande);
            }

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


}