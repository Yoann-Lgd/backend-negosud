using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class PanierExpirationService
{
    private static IServiceProvider _serviceProvider;
    private static ILogger<PanierExpirationService> _logger;

    public static void Initialize(IServiceProvider serviceProvider, ILogger<PanierExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public static async Task CleanupExpiredBasketsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
            var currentDate = DateTime.UtcNow;

            try
            {
                var expiredBaskets = await context.Commandes
                    .Where(c => !c.Valide && c.ExpirationDate < currentDate)
                    .ToListAsync();

                if (expiredBaskets.Any())
                {
                    context.Commandes.RemoveRange(expiredBaskets);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("{Count} paniers expirés ont été supprimés.", expiredBaskets.Count);
                }
                else
                {
                    _logger.LogInformation("Aucun panier expiré à supprimer.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la suppression des paniers expirés.");
            }
        }
    }
}
