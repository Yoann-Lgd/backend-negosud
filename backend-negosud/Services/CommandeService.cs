using AutoMapper;
using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Validation.Commande;
using FluentValidation.Results;

namespace backend_negosud.Services;

public class CommandeService : ICommandeService
{
    private readonly ICommandeRepository _commandeRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IFactureRepository _factureRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IReglementRepository _reglementRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommandeService> _logger;

    public CommandeService(IMapper mapper, ICommandeRepository commandeRepository, IArticleRepository articleRepository, 
        IClientRepository clientRepository, ILogger<CommandeService> logger, IReglementRepository reglementRepository, 
        IFactureRepository factureRepository, IStockRepository stockRepository)
    {
        _commandeRepository = commandeRepository;
        _clientRepository = clientRepository;
        _articleRepository = articleRepository;
        _factureRepository = factureRepository;
        _reglementRepository = reglementRepository;
        _stockRepository = stockRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<IResponseDataModel<CommandeOutputDto>> CreateCommande(CommandeInputDto commandeInput)
    {
        try
        {
            var validator = new CommandeCreateValidation();
            ValidationResult validationResult = validator.Validate(commandeInput);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors),
                };
            }

            _logger.LogInformation("Tentative de récupération du client avec ID: {ClientId}", commandeInput.ClientId);
            var client = await _clientRepository.GetByIdAsync(commandeInput.ClientId);

            if (client == null)
            {
                _logger.LogError("Client introuvable pour l'ID: {ClientId}", commandeInput.ClientId);
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    Message = "Client introuvable.",
                    StatusCode = 404
                };
            }

            var ligneCommandes = _mapper.Map<List<LigneCommande>>(commandeInput.LigneCommandes);
            foreach (var ligne in ligneCommandes)
            {
                var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == ligne.ArticleId);
                if (!articleExist)
                {
                    _logger.LogWarning("Article ID {ArticleId} n'existe pas.", ligne.ArticleId);
                }
            }

            var commande = _mapper.Map<Commande>(commandeInput);
            commande.DateCreation = DateTime.UtcNow;
            commande.LigneCommandes = ligneCommandes;

            var livraisonInputDto = commandeInput.Livraison;
            var livraison = _mapper.Map<Livraison>(livraisonInputDto);
            livraison.DateEstimee = DateTime.UtcNow.AddDays(5);
            livraison.Livree = false;

            await _commandeRepository.AddAsync(commande);

            var createdCommande = await _commandeRepository.GetByIdAndLigneCommandesAsync(commande.CommandeId);

            var outputDto = _mapper.Map<CommandeOutputDto>(createdCommande);

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = true,
                Message = "Commande créée avec succès.",
                StatusCode = 201,
                Data = outputDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création de la commande");

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la création de la commande.",
                StatusCode = 500,
                Data = null,
            };
        }
    }


    public async Task<CommandeOutputDto> GetCommandeById(int id)
    {
        var commande = await _commandeRepository.GetByIdAndLigneCommandesAsync(id);
        var commandeOutputDto = _mapper.Map<CommandeOutputDto>(commande);
        return commandeOutputDto;
    }

    public async Task<IResponseDataModel<List<CommandeOutputDto>>> GetAllCommandes()
    {
        try
        {
            var commandes = await _commandeRepository.GetAllCommandeAsync();
            
            var commandeOutputDtos = _mapper.Map<List<CommandeOutputDto>>(commandes);
            
            return new ResponseDataModel<List<CommandeOutputDto>>
            {
                Success = true,
                Message = "Commandes récupérées avec succès.",
                StatusCode = 200,
                Data = commandeOutputDtos
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des commandes");

            return new ResponseDataModel<List<CommandeOutputDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500
            };
        }
    }
    
    // fonction qui met à jour le statut de la commande à payée, et retire les articles du stock
   public async Task<BooleanResponseDataModel> UpdateOrderStatusToPaidAsync(string orderId, decimal? stripeAmount = null)
    {
        try
        {   
            if (!int.TryParse(orderId, out int commandeId))
            {
                _logger.LogError("id de commande invalide : {OrderId}", orderId);
                return new BooleanResponseDataModel
                {
                    Success = false,
                    Data = false,
                    Message = "id de commande invalide.",
                    StatusCode = 400
                };
            }
            
            // utilisation d'une transaction pour éviter les erreurs dans la bdd et faire un roolback si une action ne s'effectue pas
            using var transaction = await _commandeRepository.BeginTransactionAsync();
            
            try
            {
                var commande = await _commandeRepository.GetByIdAndLigneCommandesAsync(commandeId);
                if (commande == null)
                {
                    _logger.LogError("Commande introuvable pour l'ID : {CommandeId}", commandeId);
                    return new BooleanResponseDataModel
                    {
                        Success = false,
                        Data = false,
                        Message = "Commande introuvable.",
                        StatusCode = 404
                    };
                }

                if (commande.Valide)
                {
                    _logger.LogWarning("La commande {CommandeId} est déjà payée.", commandeId);
                    return new BooleanResponseDataModel
                    {
                        Success = true,
                        Data = true,
                        Message = "La commande est déjà payée.",
                        StatusCode = 200
                    };
                }
                
                if (commande.LigneCommandes == null || !commande.LigneCommandes.Any())
                {
                    _logger.LogWarning("La commande {CommandeId} n'a pas de lignes de commande.", commandeId);
                    return new BooleanResponseDataModel
                    {
                        Success = false,
                        Data = false,
                        Message = "La commande n'a pas de lignes de commande.",
                        StatusCode = 400
                    };
                }

                // trie des lignes valides
                var lignesValides = commande.LigneCommandes
                    .Where(lc => lc.Article != null && lc.Article.Tva != null)
                    .ToList();
                    
                if (lignesValides.Count != commande.LigneCommandes.Count)
                {
                    _logger.LogWarning("Certaines lignes de commande ont des articles ou des TVA manquants.");
                }
                
                double montantHt;
                double montantTva;
                double montantTtc;
                
                // si on a un paiement strippe
                if (stripeAmount.HasValue)
                {
                    // converitr en centimes (stripe utilise des centimes
                    montantTtc = (double)(stripeAmount.Value / 100m);
                    montantHt = Math.Round(montantTtc / 1.2, 2);
                    montantTva = Math.Round(montantTtc - montantHt, 2);
                }
                else
                {
                    // si pas de montant stripe on va utiliser les lignes de commande
                    montantHt = lignesValides.Sum(lc => lc.Article.Prix * lc.Quantite);
                    montantTva = lignesValides.Sum(lc => lc.Article.Prix * lc.Quantite * (lc.Article.Tva.Valeur / 100));
                    montantTtc = montantHt + montantTva;
                }
                
                _logger.LogInformation("Montants calculés - HT: {MontantHT}, TVA: {MontantTVA}, TTC: {MontantTTC}", 
                    montantHt, montantTva, montantTtc);
                
                // création d'une facture
                var facture = new Facture
                {
                    CommandeId = commandeId,
                    ClientId = commande.ClientId,
                    DateFacturation = DateTime.UtcNow,
                    Reference = $"FACT-{DateTime.UtcNow:yyyyMMdd}-{commandeId}",
                    MontantHt = montantHt,
                    MontantTva = montantTva,
                    MontantTtc = montantTtc
                };
                
                await _factureRepository.AddAsync(facture);
                
                // création d'un règlement pour la commande
                var reglement = new Reglement
                {
                    CommandeId = commandeId,
                    Date = DateTime.UtcNow,
                    Montant = montantTtc,
                    Reference = $"REG-STRIPE-{DateTime.UtcNow:yyyyMMddHHmmss}"
                };
                
                await _reglementRepository.AddAsync(reglement);
                
                await _commandeRepository.UpdateCommandeFieldsAsync(new Commande
                {
                    CommandeId = commandeId,
                    Valide = true,
                    FactureId = facture.FactureId
                });
                
                // mise à jour les stocks
                foreach (var ligneCommande in lignesValides)
                {
                    var stock = await _stockRepository.FirstOrDefaultAsync(s => s.ArticleId == ligneCommande.ArticleId);
                    if (stock != null)
                    {
                        stock.Quantite -= ligneCommande.Quantite;
                        await _stockRepository.UpdateAsync(stock);
                    }
                    else
                    {
                        _logger.LogWarning("Stock non trouvé pour l'article {ArticleId}", ligneCommande.ArticleId);
                    }
                }
                
                await transaction.CommitAsync();
                
                return new BooleanResponseDataModel
                {
                    Success = true,
                    Data = true,
                    Message = "La commande a été validée et le paiement enregistré avec succès.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de la commande {OrderId} comme payée", orderId);
            return new BooleanResponseDataModel
            {
                Success = false,
                Data = false,
                Message = $"Erreur lors de la mise à jour de la commande : {ex.Message}",
                StatusCode = 500
            };
        }
    }

    public async Task<IResponseDataModel<string>> SoftDeleteAsync(int commandeId)
    {
        try
        {
            if (commandeId <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer la commande : {Id}", commandeId);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }

            var commande = await _commandeRepository.GetByIdAsync(commandeId);
            if (commande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }

            if (!commande.Valide)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande est à l'état de panier.",
                    StatusCode = 400,
                    Data = commandeId.ToString(),
                };
            }

            var response = await _commandeRepository.SoftDeleteEntityByIdAsync<Commande, int>(commandeId);
            if (response)
            {
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "La commande a été soft-supprimée.",
                    StatusCode = 200,
                    Data = commandeId.ToString(),
                };
            }
            else
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du soft delete de la commande");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors du soft delete de la commande.",
                StatusCode = 500,
            };
        }
    }

}