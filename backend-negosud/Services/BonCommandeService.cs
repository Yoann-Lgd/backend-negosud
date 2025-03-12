using AutoMapper;
using backend_negosud.DTOs.Commande_fournisseur.Inputs;
using backend_negosud.DTOs.Commande_fournisseur.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Validation.BonCommande;
using FluentValidation.Results;

namespace backend_negosud.Services;

public class BonCommandeService : IBonCommandeService
{
    private readonly IMapper _mapper;
    private readonly IBonCommandeRepository _bonCommandeRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly ILogger<BonCommandeService> _logger;
    private readonly IFournisseurRepository _fournisseurRepository;
    public BonCommandeService(IMapper mapper, IBonCommandeRepository bonCommandeRepository, IArticleRepository articleRepository, 
        IUtilisateurRepository utilisateurRepository, ILogger<BonCommandeService> logger, IFournisseurRepository fournisseurRepository)
    {
        _mapper = mapper;
        _bonCommandeRepository = bonCommandeRepository;
        _articleRepository = articleRepository;
        _utilisateurRepository = utilisateurRepository;
        _logger = logger;
        _fournisseurRepository = fournisseurRepository;
    }
    
    public async Task<BooleanResponseDataModel> CreateBonCommande(BonCommandeCreateInputDto bonCommandeInput)
    {
        try
        {
            var validator = new BonCommandeCreateValidation();
            ValidationResult validationResult = validator.Validate(bonCommandeInput);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                return new BooleanResponseDataModel
                {
                    Data = false,
                    Success = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors),
                };
            }
            
            _logger.LogInformation("Tentative de récupération du fournisseur avec ID: {FournisseurId}", bonCommandeInput.FournisseurID);
            var fournisseur = await _fournisseurRepository.GetByIdAsync(bonCommandeInput.FournisseurID);

            if (fournisseur == null)
            {
                _logger.LogError("Fournisseur introuvable pour l'ID: {FournisseurId}", bonCommandeInput.FournisseurID);
                return new BooleanResponseDataModel
                {
                    Data = false,
                    Success = false,
                    Message = "Fournisseur introuvable.",
                    StatusCode = 404
                };
            }
            
            _logger.LogInformation("Tentative de récupération de l'utilisateur avec ID: {UtilisateurId}", bonCommandeInput.UtilisateurId);
            var utilisateur = await _utilisateurRepository.GetByIdAsync(bonCommandeInput.UtilisateurId);

            if (utilisateur == null)
            {
                _logger.LogError("Utilisateur introuvable pour l'ID: {UtilisateurId}", bonCommandeInput.UtilisateurId);
                return new BooleanResponseDataModel
                {
                    Data = false,
                    Success = false,
                    Message = "Utilisateur introuvable.",
                    StatusCode = 404
                };
            }
            
            var ligneBonCommandes = new List<LigneBonCommande>();
            foreach (var ligneDto in bonCommandeInput.LigneCommandes)
            {
                var article = await _articleRepository.GetByIdAsync(ligneDto.ArticleId);
                    
                if (article == null)
                {
                    _logger.LogWarning("Article ID {ArticleId} n'existe pas.", ligneDto.ArticleId);
                    return new BooleanResponseDataModel
                    {
                        Data = false,
                        Success = false,
                        Message = $"L'article avec l'ID {ligneDto.ArticleId} n'existe pas.",
                        StatusCode = 404
                    };
                }
                    
                if (article.FournisseurId != bonCommandeInput.FournisseurID)
                {
                    _logger.LogWarning("L'article ID {ArticleId} n'appartient pas au fournisseur ID {FournisseurId}.", 
                        ligneDto.ArticleId, bonCommandeInput.FournisseurID);
                    return new BooleanResponseDataModel
                    {
                        Data = false,
                        Success = false,
                        Message = $"L'article avec l'ID {ligneDto.ArticleId} n'appartient pas au fournisseur spécifié.",
                        StatusCode = 400
                    };
                }

                var ligne = new LigneBonCommande
                {
                    ArticleId = ligneDto.ArticleId,
                    Quantite = ligneDto.Quantite,
                    PrixUnitaire = ligneDto.PrixUnitaire,
                    Livree = false
                };
                
                ligneBonCommandes.Add(ligne);
            }
            
            var bonCommande = new BonCommande
            {
                FournisseurId = bonCommandeInput.FournisseurID,
                UtilisateurId = bonCommandeInput.UtilisateurId,
                Reference = $"BC-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                Status = "En attente",
                Prix = bonCommandeInput.Prix <= 0 
                    ? ligneBonCommandes.Sum(l => l.PrixUnitaire * l.Quantite) 
                    : bonCommandeInput.Prix,
                LigneBonCommandes = ligneBonCommandes
            };
            
            await _bonCommandeRepository.AddAsync(bonCommande);

            return new BooleanResponseDataModel
            {
                Data = true,
                Success = true,
                Message = "Bon de commande créé avec succès.",
                StatusCode = 201
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création du bon de commande");

            return new BooleanResponseDataModel
            {
                Data = false,
                Success = false,
                Message = "Une erreur s'est produite lors de la création du bon de commande.",
                StatusCode = 500
            };
        }
    }

    public async Task<IResponseDataModel<List<BonCommandeOutputDto>>> GetAllBonCommandes()
    {
        try
        {
            var bonCommandes = await _bonCommandeRepository.GetAllCommandeAsync();
            
            var bonCommandeOutputDtos = _mapper.Map<List<BonCommandeOutputDto>>(bonCommandes);
            
            return new ResponseDataModel<List<BonCommandeOutputDto>>
            {
                Success = true,
                Message = "Commandes récupérées avec succès.",
                StatusCode = 200,
                Data = bonCommandeOutputDtos
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des commandes");

            return new ResponseDataModel<List<BonCommandeOutputDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500,
            };
        }
    }

    public async Task<IResponseDataModel<BonCommandeOutputDto>> GetBonCommandeById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour récupérer la commande : {Id}", id);
                return new ResponseDataModel<BonCommandeOutputDto>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }
            
            var bonCommande = await _bonCommandeRepository.GetById(id);
            
            var bonCommandeOutputDto = _mapper.Map<BonCommandeOutputDto>(bonCommande);
            
            return new ResponseDataModel<BonCommandeOutputDto>
            {
                Success = true,
                Message = "Commande récupérée avec succès.",
                StatusCode = 200,
                Data = bonCommandeOutputDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des commandes");

            return new ResponseDataModel<BonCommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500,
            };
        }
    }
}