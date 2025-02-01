using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class StockService : ControllerBase, IStockService
{
    private readonly PostgresContext _context;

    public StockService(PostgresContext context)
    {
        _context = context;
    }
    
    public async Task<IResponseDataModel<Stock>> AddArticleToStock(int articleId, int quantite, string refLot, int seuilMinimum, bool reapprovisionnementAuto)
    {
        var article = await _context.Articles.FindAsync(articleId); // TODO: quand article repository fini, appeler findarticlebyId ici
        if (article == null)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Article non trouvé"
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

        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync();

        return new ResponseDataModel<Stock>
        {
            Success = true,
            Message = "Article ajouté au stock avec succès"
        };
    }

    public async Task<IResponseDataModel<Stock>>  UpdateStockQuantity(int stockId, int nouvelleQuantite, int utilisateurId, string typeModification)
    {
        var stock = await _context.Stocks.FindAsync(stockId);
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

        // Enregistrer l'historique dans la table Inventorier
        var inventorier = new Inventorier
        {
            UtilisateurId = utilisateurId,
            StockId = stockId,
            QuantitePrecedente = ancienneQuantite,
            QuantitePostModification = nouvelleQuantite,
            TypeModification = typeModification,
            DateModification = DateTime.UtcNow
        };

        _context.Inventoriers.Add(inventorier);
        await _context.SaveChangesAsync();

        return new ResponseDataModel<Stock>
        {
            Success = true,
            Message = "Stock mis à jour."
        };
    }

    public async Task<IResponseDataModel<Stock>>  CheckStockLevel(int articleId, int quantiteDemandee)
    {
        var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ArticleId == articleId);
        if (stock == null)
        {
            return new ResponseDataModel<Stock>
            {
                Success = false,
                Message = "Stock non trouvé pour cet article."
            };
        }

        if (stock.Quantite < quantiteDemandee)
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

    public async Task<IResponseDataModel<Stock>>  CheckAndReapprovisionner()
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

    public async Task<IResponseDataModel<List<Inventorier>>>  GetStockHistory(int stockId)
    {
        var history = await _context.Inventoriers
            .Where(i => i.StockId == stockId)
            .OrderByDescending(i => i.DateModification)
            .ToListAsync();

        return new ResponseDataModel<List<Inventorier>>
        {
            Success = true,
            Message = "Historique récupéré avec succès.",
            Data = history
        };
    }
}