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
    private readonly ILigneBonCommandeRepository _ligneBonCommandeRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly ILogger<BonCommandeService> _logger;
    private readonly IStockService _stockService;
    private readonly IFournisseurRepository _fournisseurRepository;
    public BonCommandeService(IMapper mapper, IBonCommandeRepository bonCommandeRepository, IArticleRepository articleRepository, IStockService stockService,
        ILigneBonCommandeRepository ligneCommandeRepository, IUtilisateurRepository utilisateurRepository, ILogger<BonCommandeService> logger, IFournisseurRepository fournisseurRepository)
    {
        _mapper = mapper;
        _stockService = stockService;
        _bonCommandeRepository = bonCommandeRepository;
        _articleRepository = articleRepository;
        _utilisateurRepository = utilisateurRepository;
        _logger = logger;
        _fournisseurRepository = fournisseurRepository;
        _ligneBonCommandeRepository = ligneCommandeRepository;
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
                DateCreation = DateTime.UtcNow,
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

    public async Task<IResponseDataModel<BonCommandeOutputDto>> UpdateBonCommande(int id, BonCommandeUpdateDto bonCommandeUpdateInput)
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
            if (bonCommande == null)
            {
                _logger.LogError("Commande fournisseur introuvable pour l'ID: {BonCommandeId}", id);
                return new ResponseDataModel<BonCommandeOutputDto>
                {
                    Success = false,
                    Message = "Commande fournisseur introuvable.",
                    StatusCode = 404
                };
            }
            string ancienStatus = bonCommande.Status;
            
            bonCommande.Status = bonCommandeUpdateInput.Status;
            
            // on vérifie si la liste des lignes est vide ou null
            if (bonCommandeUpdateInput.LigneCommandes == null || !bonCommandeUpdateInput.LigneCommandes.Any())
            {
                _logger.LogInformation("Aucune ligne de commande spécifiée, conservation des lignes existantes.");
            }
            else
            {
                // id des lignes présentes dans le DTO d'entrée
                var updateDtoLigneIds = bonCommandeUpdateInput.LigneCommandes
                    .Where(l => l.LigneBonCommandeId > 0) // Filtrer pour obtenir seulement les lignes existantes
                    .Select(l => l.LigneBonCommandeId)
                    .ToHashSet();
                    
                // suppression des lignes qui ne sont plus présentes dans le DTO d'entrée
                var lignesToRemove = bonCommande.LigneBonCommandes
                    .Where(l => !updateDtoLigneIds.Contains(l.LigneBonCommandeId))
                    .ToList();
                    
                foreach (var ligne in lignesToRemove)
                {
                    _logger.LogInformation("Suppression de la ligne de commande {LigneId} de la commande {CommandeId}", 
                        ligne.LigneBonCommandeId, id);
                    bonCommande.LigneBonCommandes.Remove(ligne);
                }
                
                // mise à jour les lignes existantes
                foreach (var ligneUpdateDto in bonCommandeUpdateInput.LigneCommandes.Where(l => l.LigneBonCommandeId > 0))
                {
                    var existingLigne = bonCommande.LigneBonCommandes
                        .FirstOrDefault(l => l.LigneBonCommandeId == ligneUpdateDto.LigneBonCommandeId);
                        
                    if (existingLigne != null)
                    {
                        // mise à jour uniquement la quantité et le statut livré
                        existingLigne.Quantite = ligneUpdateDto.Quantite;
                        existingLigne.Livree = ligneUpdateDto.Livree;
                    }
                    else
                    {
                        _logger.LogWarning("Ligne de commande non trouvée : {LigneId} pour la commande {CommandeId}", 
                            ligneUpdateDto.LigneBonCommandeId, id);
                    }
                }
                
                // ajout des nouvelles lignes de commande
                var newLines = bonCommandeUpdateInput.LigneCommandes
                    .Where(l => l.LigneBonCommandeId <= 0)
                    .ToList();

                foreach (var newLineDto in newLines)
                {
                    var article = await _articleRepository.GetByIdAsync(newLineDto.ArticleId);
                    if (article == null)
                    {
                        _logger.LogWarning("Article non trouvé pour l'ID: {ArticleId}", newLineDto.ArticleId);
                        continue; // Ignorez cette ligne si l'article n'existe pas
                    }
                    
                    // création d'une nouvelle ligne de commande
                    var newLine = new LigneBonCommande
                    {
                        ArticleId = newLineDto.ArticleId,
                        BonCommandeId = bonCommande.BonCommandeId,
                        Quantite = newLineDto.Quantite,
                        PrixUnitaire = article.Prix,
                        Livree = newLineDto.Livree
                    };
                    
                    // ajout à la collection
                    bonCommande.LigneBonCommandes.Add(newLine);
                    _logger.LogInformation("Ajout d'une nouvelle ligne pour l'article {ArticleId} à la commande {CommandeId}", 
                        newLineDto.ArticleId, id);
                }
            }
            
            // recalcul du prix total en fonction des nouvelles quantités
            double nouveauPrix = bonCommande.LigneBonCommandes.Sum(l => l.PrixUnitaire * l.Quantite);
            bonCommande.Prix = nouveauPrix;
            
            await _bonCommandeRepository.UpdateAsync(bonCommande);
            
            // vérification du statut, s'il est bien à Livrée, quu'il n'était déjà pas à ce statut et que toutes les ligne de commandes ont livree a true
            if (bonCommande.Status == "Livrée" && bonCommande.LigneBonCommandes.All(l => l.Livree) && ancienStatus != "Livrée")
            {
                
                var reapproResult = await _stockService.ReapprovisionnerStockDepuisBonCommande(id, bonCommandeUpdateInput.UtilisateurId);
                
                if (!reapproResult.Success)
                {
                    _logger.LogWarning("La mise à jour du bon de commande a réussi mais le réapprovisionnement du stock a échoué: {Message}", 
                        reapproResult.Message);
                }
            }
                
            // rçupère la commande mise à jour avec toutes ses relations
            var updatedBonCommande = await _bonCommandeRepository.GetById(id);
            var outputDto = _mapper.Map<BonCommandeOutputDto>(updatedBonCommande);
            
            return new ResponseDataModel<BonCommandeOutputDto>
            {
                Success = true,
                Message = "Commande fournisseur mise à jour avec succès.",
                StatusCode = 200,
                Data = outputDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la mise à jour de la commande fournisseur");
            
            return new ResponseDataModel<BonCommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la mise à jour de la commande fournisseur.",
                StatusCode = 500,
                Data = null
            };
        }
    }
    
    public async Task<BooleanResponseDataModel> DeleteLigneCommande(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer la ligne de commande : {Id}", id);
                return new BooleanResponseDataModel
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                    Data = false
                };
            }
            
            var ligneBonCommande = await _ligneBonCommandeRepository.GetByIdAsync(id);
            if (ligneBonCommande == null)
            {
                _logger.LogError("Commande fournisseur introuvable pour l'ID: {BonCommandeId}", id);
                return new BooleanResponseDataModel()
                {
                    Success = false,
                    Message = "Ligne de la commande fournisseur introuvable.",
                    StatusCode = 404,
                    Data = false
                };
            }
            
            await _ligneBonCommandeRepository.DeleteAsync(ligneBonCommande);
            
            return new BooleanResponseDataModel()
            {
                Success = true,
                Message = "Ligne de commande fournisseur supprimée avec succès.",
                StatusCode = 200,
                Data = true
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression de la ligne de commande fournisseur");
            
            return new BooleanResponseDataModel
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression de la ligne de commande.",
                StatusCode = 500,
                Data = false
            };
        }
    }
}