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
    
    public async Task<IResponseDataModel<PanierCreateOutputDto>> CreatePanier(PanierCreateInputDto panierCreateInputDto)
    {
        try
        {
            if (panierCreateInputDto.ClientId <= 0)
            {
                _logger.LogError("ClientId est invalide.");
                return new ResponseDataModel<PanierCreateOutputDto>
                {
                    Success = false,
                    Message = "ClientId invalide."
                };
            }
            
            _logger.LogInformation("Tentative de récupération du client avec ID: {ClientId}", panierCreateInputDto.ClientId);
            var client = await _clientRepository.GetByIdAsync(panierCreateInputDto.ClientId);
            
            if (client == null)
            {
                _logger.LogError("Client introuvable pour l'ID: {ClientId}", panierCreateInputDto.ClientId);
                return new ResponseDataModel<PanierCreateOutputDto>
                {
                    Success = false,
                    Message = "Client introuvable.",
                    StatusCode = 404
                };
            }
            
            var ligneCommandes = _mapper.Map<List<LigneCommande>>(panierCreateInputDto.LigneCommandes);
            foreach (var ligne in ligneCommandes)
            {
                var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == ligne.ArticleId);
                if (!articleExist)
                {
                    _logger.LogWarning("Article ID {ArticleId} n'existe pas.", ligne.ArticleId);
                }
            }

            var commande = _mapper.Map<Commande>(panierCreateInputDto);
            commande.LigneCommandes = ligneCommandes;
            
            await _commandeRepository.AddAsync(commande);
            
            var outputDto = _mapper.Map<PanierCreateOutputDto>(commande);

            return new ResponseDataModel<PanierCreateOutputDto>
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

            return new ResponseDataModel<PanierCreateOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500,
                Data = null,
            };
        }
    }
}