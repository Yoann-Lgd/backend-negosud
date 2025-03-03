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

    public StockService(IMapper mapper, IStockRepository stockRepository, IArticleRepository articleRepository, IBonCommandeRepository bonCommandeRepository, IInventorierRepository inventorierRepository, PostgresContext context, ILogger<StockService> logger)
    {
        _stockRepository = stockRepository;
        _articleRepository = articleRepository;
        _bonCommandeRepository = bonCommandeRepository;
        _inventorierRepository = inventorierRepository;
        _context = context;
        _mapper = mapper;
        _logger = logger;
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
        var stocks = await _stockRepository.GetAllAsync();
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

    // TODO : Automatiser comme le fait de vider le panier automatiquement.
    public async Task<IResponseDataModel<Stock>> CheckAndReapprovisionner()
    {
        var stocksAReapprovisionner = await _context.Stocks
            .Where(s => s.Quantite <= s.SeuilMinimum && s.ReapprovisionnementAuto)
            .ToListAsync();

        foreach (var stock in stocksAReapprovisionner)
        {
            // Créer un bon de commande pour le fournisseur
            var bonCommande = new BonCommande
            {
                Reference = $"BC-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                Status = "En attente",
                UtilisateurId = 1, // On image que le user ID 1 gère le stock
                Prix = 0 // À calculer en fonction des articles commandés
            };

            _context.BonCommandes.Add(bonCommande);
            await _context.SaveChangesAsync();

            // Ajouter une ligne de bon de commande
            var ligneBonCommande = new LigneBonCommande
            {
                ArticleId = stock.ArticleId,
                BonCommandeId = bonCommande.BonCommandeId,
                Quantite = stock.SeuilMinimum * 2, // On imagine qu'on commande le double du seuil minimum
                PrixUnitaire = await _context.Articles
                    .Where(a => a.ArticleId == stock.ArticleId)
                    .Select(a => a.Prix)
                    .FirstOrDefaultAsync()
            };

            _context.LigneBonCommandes.Add(ligneBonCommande);
            await _context.SaveChangesAsync();
        }

        return new ResponseDataModel<Stock>
        {
            Success = true,
            Message = "Réapprovisionnement automatique effectué."
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

}