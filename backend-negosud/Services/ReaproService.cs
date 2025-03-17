using backend_negosud.Entities;
using backend_negosud.Repository;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class ReaproService
{
    private static IServiceProvider _serviceProvider;
    private static ILogger<ReaproService> _logger;

    public static void Initialize(IServiceProvider serviceProvider, ILogger<ReaproService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public static async Task CheckAndReapprovisionnerAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
            var bonCommandeRepository = scope.ServiceProvider.GetRequiredService<IBonCommandeRepository>();
            var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();

            try
            {
                //  on récupère tous les articles qui ont déjà une commande en cours (non livrée)
                var articlesEnCommande = await context.LigneBonCommandes
                    .Include(l => l.BonCommande) 
                    .Where(l => !l.Livree && 
                              (l.BonCommande.Status == "En attente" || 
                               l.BonCommande.Status == "Validée"))
                    .Select(l => l.ArticleId)
                    .Distinct()
                    .ToListAsync();

                // on récupère les stocks à réapprovisionner, et on exclut ceux qui ont déjà une commande en cours
                var stocksAReapprovisionner = await context.Stocks
                    .Include(s => s.Article)
                    .ThenInclude(a => a.Fournisseur)
                    .Where(s => s.Quantite <= s.SeuilMinimum && 
                              s.ReapprovisionnementAuto && 
                              !articlesEnCommande.Contains(s.ArticleId))
                    .ToListAsync();

                if (!stocksAReapprovisionner.Any())
                {
                    _logger.LogInformation("Aucun stock à réapprovisionner automatiquement.");
                    return;
                }

                // on regroupe les stocks par fournisseur
                var stocksParFournisseur = stocksAReapprovisionner
                    .GroupBy(s => s.Article.FournisseurId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var kvp in stocksParFournisseur)
                {
                    int fournisseurId = kvp.Key;
                    var stocksDuFournisseur = kvp.Value;

                    // Préparer les lignes de commande
                    var lignesBonCommande = new List<LigneBonCommande>();
                    double prixTotal = 0;

                    foreach (var stock in stocksDuFournisseur)
                    {
                        var article = await articleRepository.GetByIdAsync(stock.ArticleId);
                        int quantiteACommander = stock.SeuilMinimum * 2; // Double du seuil minimum
                        double prixUnitaire = article.Prix;
                        
                        var ligneBonCommande = new LigneBonCommande
                        {
                            ArticleId = stock.ArticleId,
                            Quantite = quantiteACommander,
                            PrixUnitaire = prixUnitaire,
                            Livree = false
                        };

                        lignesBonCommande.Add(ligneBonCommande);
                        prixTotal += prixUnitaire * quantiteACommander;
                    }

                    // création du bon de commande
                    var bonCommande = new BonCommande
                    {
                        Reference = $"BC-AUTO-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                        Status = "En attente",
                        DateCreation = DateTime.UtcNow,
                        UtilisateurId = 1, // Utilisateur système pour l'automatisation
                        FournisseurId = fournisseurId,
                        Prix = prixTotal,
                        LigneBonCommandes = lignesBonCommande
                    };

                    // Sauvegarder le bon de commande avec ses lignes
                    await bonCommandeRepository.AddAsync(bonCommande);

                    _logger.LogInformation(
                        "Bon de commande automatique créé pour le fournisseur ID {FournisseurId} avec {Count} articles pour un total de {Total}€.",
                        fournisseurId, lignesBonCommande.Count, prixTotal);
                }

                _logger.LogInformation("{Count} articles ont été réapprovisionnés automatiquement pour {FournisseurCount} fournisseurs.",
                    stocksAReapprovisionner.Count, stocksParFournisseur.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors du réapprovisionnement automatique des stocks.");
            }
        }
    }
}
