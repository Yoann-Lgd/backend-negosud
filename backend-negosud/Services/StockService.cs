using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class StockService : IStockService
{
    private IStockRepository _stockRepository;
    private IArticleRepository _articleRepository;
    private IBonCommandeRepository _bonCommandeRepository;
    private ILigneBonCommandeRepository _ligneBonCommandeRepository;
    private IInventorierRepository _inventorierRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StockService> _logger;
    private readonly PostgresContext _context;
    private readonly IServiceProvider _serviceProvider;

    public StockService(IServiceProvider serviceProvider,IMapper mapper, IStockRepository stockRepository, IArticleRepository articleRepository, IBonCommandeRepository bonCommandeRepository, IInventorierRepository inventorierRepository, PostgresContext context, ILogger<StockService> logger)
    {
        _stockRepository = stockRepository;
        _articleRepository = articleRepository;
        _bonCommandeRepository = bonCommandeRepository;
        _inventorierRepository = inventorierRepository;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<IResponseDataModel<Stock>> AddArticleToStock(int articleId, int quantite, string refLot,
        int seuilMinimum, bool reapprovisionnementAuto)
    {
        var article = await _articleRepository.GetByIdAsync(articleId); // TODO: quand article repository fini, appeler findarticlebyId ici
        if (article == null)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Article non trouvé"
            };
        }

        var existingStock = await _stockRepository.FirstOrDefaultAsync(s => s.ArticleId == articleId);
        if (existingStock != null)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Un stock avec cet article existe déjà"
            };
        }

        var stock = new Stock
        {
            ArticleId = articleId,
            Quantite = quantite,
            RefLot = refLot,
            SeuilMinimum = seuilMinimum,
            ReapprovisionnementAuto = reapprovisionnementAuto
        };
    
        var stockCreated = await _stockRepository.AddAsync(stock);

        return new ResponseDataModel<Stock>
        {
            Success = true,
            Message = "Article ajouté au stock avec succès",
            Data = stockCreated,
        };
    }

    public async Task<IResponseDataModel<Stock>> UpdateStockQuantity(int stockId, int nouvelleQuantite, int utilisateurId, string typeModification)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(); // Les transactions permettent à plusieurs opérations de base de données d’être traitées de manière atomique. Utile ici comme on a plusieurs saveChange
        try
        {
            var stock = await _stockRepository.GetByIdAsync(stockId);
            if (stock == null)
            {
                return new ResponseDataModel<Stock>
                {
                    Success = false,
                    Message = "Stock non trouvé"
                };
            }

            // Enregistrer l'ancienne quantité
            int ancienneQuantite = stock.Quantite;

            // Mettre à jour la quantité
            stock.Quantite = nouvelleQuantite;
            await _stockRepository.UpdateAsync(stock);

            var existingEntry = await _context.Inventoriers
                .FirstOrDefaultAsync(i => i.UtilisateurId == utilisateurId && i.StockId == stockId);

            if (existingEntry != null)
            {
                _context.Inventoriers.Remove(existingEntry);
                await _context.SaveChangesAsync();
            }

// Créer une nouvelle entrée
            var inventorier = new Inventorier
            {
                UtilisateurId = utilisateurId,
                StockId = stockId,
                QuantitePrecedente = ancienneQuantite,
                QuantitePostModification = nouvelleQuantite,
                TypeModification = typeModification,
                DateModification = DateTime.UtcNow
            };

            await _inventorierRepository.AddAsync(inventorier);

            await transaction.CommitAsync();

            return new ResponseDataModel<Stock>
            {
                Success = true,
                Message = "Stock mis à jour avec succès"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = $"Erreur lors de la mise à jour du stock : {ex.Message}"
            };
        }
    }

    public async Task<IResponseDataModel<Stock>> PatchStock(int stockId, StockInputPatchDto stockInputDto)
    {
        try
        {
            var stock = await _stockRepository.FirstOrDefaultAsync(s => s.StockId == stockId);
            if (stock == null)
            {
                return new ResponseDataModel<Stock>
                {
                    Success = false,
                    Message = "Stock non trouvé"
                };
            }

            if (!string.IsNullOrEmpty(stockInputDto.RefLot))
            {
                stock.RefLot = stockInputDto.RefLot;
            }
        
            if (stockInputDto.SeuilMinimum != 0)
            {
                stock.SeuilMinimum = stockInputDto.SeuilMinimum;
            }

            if (stockInputDto.ReapprovisionnementAuto != null)
            {
                stock.ReapprovisionnementAuto = stockInputDto.ReapprovisionnementAuto;
            }

            if (stockInputDto.Quantite != 0)
            {
                stock.Quantite = stockInputDto.Quantite;
            }
        
            await _stockRepository.UpdateAsync(stock);

            return new ResponseDataModel<Stock>()
            {
                Success = true,
                StatusCode = 200,
                Message = "Le stock a été mis à jour avec succès"
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

    }

    public async Task<IResponseDataModel<Stock>>  CheckStockLevel(int articleId)
    {
        var stock = await _stockRepository.FirstOrDefaultAsync(s => s.ArticleId == articleId);
        if (stock == null)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Stock non trouvé pour cet article."
            };
        }

        if (stock.Quantite < stock.SeuilMinimum)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Stock insuffisant."
            };
        }

        return new ResponseDataModel<Stock>
        {
            Success = true,
            Message = "Stock suffisant."
        };
    }

    public async Task<IResponseDataModel<List<StockSummaryDto>>> GetAllStocks()
    {
        var stocks = await _stockRepository.GetAllStocksWithArticles();
        var stockDtos = _mapper.Map<List<StockSummaryDto>>(stocks);
        return new ResponseDataModel<List<StockSummaryDto>>
        {
            Success = true,
            Data = stockDtos,
        };
    }

    public async Task<IResponseDataModel<Stock>> GetById(int id)
    {
        var stock = await _stockRepository.GetByIdAsync(id);
        return new ResponseDataModel<Stock>
        {
            Success = true,
            Data = stock,
        };
    }    
    
    public async Task<IResponseDataModel<String>> Delete(Stock stock)
    {
        await _stockRepository.DeleteAsync(stock);
        return new ResponseDataModel<String>()
        {
            Success = true,
            Data = stock.StockId.ToString()
        };
    }





    public async Task<IResponseDataModel<List<Inventorier>>> GetStockHistory(int stockId)
    {
        var history = await _inventorierRepository.GetInventoriersByStockIdAsync(stockId);

        if (history == null || !history.Any())
        {
            return new ResponseDataModel<List<Inventorier>>
            {
                Success = false,
                Message = "Aucun historique trouvé pour ce stock."
            };
        }

        return new ResponseDataModel<List<Inventorier>>
        {
            Success = true,
            Message = "Historique récupéré avec succès.",
            Data = history
        };
    }
    public async Task<IResponseDataModel<string>> ReapprovisionnerStockDepuisBonCommande(int bonCommandeId, int utilisateurId)
    {
        try
        {
            var bonCommande = await _bonCommandeRepository.GetById(bonCommandeId);
            if (bonCommande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Bon de commande non trouvé",
                    StatusCode = 404
                };
            }

            // vérifie si le statut est égal à "Livrée"
            if (bonCommande.Status != "Livrée")
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le bon de commande n'est pas au statut 'Livrée'",
                    StatusCode = 400
                };
            }

            // cehck si toutes les lignes sont marquées comme livrées
            if (bonCommande.LigneBonCommandes.Any(l => !l.Livree))
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Toutes les lignes du bon de commande ne sont pas marquées comme livrées",
                    StatusCode = 400
                };
            }

            // utiliser un système de transaction pour s'assurer que toutes les mises à jour du stock réussissent ensemble
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // mis à jour du stock de l'article pour chaque ligne de commande
                foreach (var ligne in bonCommande.LigneBonCommandes)
                {
                    var stock = await _stockRepository.FirstOrDefaultAsync(s => s.ArticleId == ligne.ArticleId);
                    
                    if (stock == null)
                    {
                        // si le stock n'existe pas, en créer un nouveau
                        stock = new Stock
                        {
                            ArticleId = ligne.ArticleId,
                            Quantite = ligne.Quantite,
                            RefLot = $"BC-{bonCommande.Reference}-{DateTime.UtcNow:yyyyMMdd}",
                            SeuilMinimum = 5, //valeur par défaut
                            ReapprovisionnementAuto = true
                        };
                        
                        await _stockRepository.AddAsync(stock);
                        
                        
                        var inventorier = new Inventorier
                        {
                            UtilisateurId = utilisateurId,
                            StockId = stock.StockId,
                            QuantitePrecedente = 0,
                            QuantitePostModification = ligne.Quantite,
                            TypeModification = "Réapprovisionnement après la livraison d'une commande d'un fournisseur",
                            DateModification = DateTime.UtcNow
                        };
                        
                        await _inventorierRepository.AddAsync(inventorier);
                    }
                    else
                    {
                        // si le stock existe, mettre à jour la quantité
                        int ancienneQuantite = stock.Quantite;
                        stock.Quantite += ligne.Quantite;
                        
                        await _stockRepository.UpdateAsync(stock);
                        
                        // création d'une entrée d'historique
                        var inventorier = new Inventorier
                        {
                            UtilisateurId = utilisateurId,
                            StockId = stock.StockId,
                            QuantitePrecedente = ancienneQuantite,
                            QuantitePostModification = stock.Quantite,
                            TypeModification = "Réapprovisionnement après la livraison d'une commande d'un fournisseur",
                            DateModification = DateTime.UtcNow
                        };
                        
                        await _inventorierRepository.AddAsync(inventorier);
                    }
                }
                
                // on valide la transaction
                await transaction.CommitAsync();
                
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Stock réapprovisionné avec succès depuis le bon de commande",
                    StatusCode = 200,
                    Data = bonCommande.BonCommandeId.ToString()
                };
            }
            catch (Exception ex)
            {
                // roolback si une erreur est survenue
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erreur lors du réapprovisionnement du stock depuis le bon de commande {BonCommandeId}", bonCommandeId);
                
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = $"Erreur lors du réapprovisionnement du stock : {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du réapprovisionnement du stock depuis le bon de commande {BonCommandeId}", bonCommandeId);
            
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = $"Erreur lors du réapprovisionnement du stock : {ex.Message}",
                StatusCode = 500
            };
        }
    }
}