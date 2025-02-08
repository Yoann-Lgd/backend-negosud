using backend_negosud.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Extentions;

public static class HangFireExtension
{
    public static void UseHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard();
        app.UseHangfireServer();

        // tâche récurrente pour nettoyer les paniers expirés
        using (var serviceScope = app.Services.CreateScope())
        {
            var recurringJobManager = serviceScope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobManager.AddOrUpdate(
                "cleanup-expired-baskets",
                () => CleanupExpiredBaskets(serviceScope.ServiceProvider),
                "*/30 * * * *"); // Exéc toutes les 30 minutes
        }
    }

    public static async Task CleanupExpiredBaskets(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
            var currentDate = DateTime.UtcNow;

            var expiredBaskets = await context.Commandes
                .Where(c => !c.Valide && c.ExpirationDate < currentDate)
                .ToListAsync();

            context.Commandes.RemoveRange(expiredBaskets);
            await context.SaveChangesAsync();
        }
    }
}