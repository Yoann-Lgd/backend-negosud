using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class ReaproService
{
    private static IServiceProvider _serviceProvider;
    private static ILogger<PanierExpirationService> _logger;

    public static void Initialize(IServiceProvider serviceProvider, ILogger<PanierExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public static async Task CheckAndReapprovisionnerAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();

            try
            {
                var stocksAReapprovisionner = await context.Stocks
                    .Where(s => s.Quantite <= s.SeuilMinimum && s.ReapprovisionnementAuto)
                    .ToListAsync();

                if (stocksAReapprovisionner.Any())
                {
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

                        context.BonCommandes.Add(bonCommande);
                        await context.SaveChangesAsync();

                        // Ajouter une ligne de bon de commande
                        var ligneBonCommande = new LigneBonCommande
                        {
                            ArticleId = stock.ArticleId,
                            BonCommandeId = bonCommande.BonCommandeId,
                            Quantite = stock.SeuilMinimum * 2, // On imagine qu'on commande le double du seuil minimum
                            PrixUnitaire = await context.Articles
                                .Where(a => a.ArticleId == stock.ArticleId)
                                .Select(a => a.Prix)
                                .FirstOrDefaultAsync()
                        };

                        context.LigneBonCommandes.Add(ligneBonCommande);
                        await context.SaveChangesAsync();
                    }

                    _logger.LogInformation("{Count} articles ont été réapprovisionnés automatiquement.",
                        stocksAReapprovisionner.Count);
                }
                else
                {
                    _logger.LogInformation("Aucun stock à réapprovisionner automatiquement.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors du réapprovisionnement automatique des stocks.");
            }
        }
    }
}
