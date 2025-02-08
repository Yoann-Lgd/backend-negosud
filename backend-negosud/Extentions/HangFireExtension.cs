using backend_negosud.Entities;
using backend_negosud.Services;
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
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate(
            "cleanup-expired-baskets",
            () => CleanupExpiredBaskets(app.Configuration.GetConnectionString("DefaultConnection")),
            "*/30 * * * *"); // Exéc toutes les 30 minutes
    }

    public static void CleanupExpiredBaskets(string connectionString)
    {
        using (var scope = CreateScope(connectionString))
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
            var currentDate = DateTime.UtcNow;

            var expiredBaskets = context.Commandes
                .Where(c => !c.Valide && c.ExpirationDate < currentDate)
                .ToList();

            context.Commandes.RemoveRange(expiredBaskets);
            context.SaveChanges();
        }
    }

    private static IServiceScope CreateScope(string connectionString)
    {
        var serviceProvider = BuildServiceProvider(connectionString);
        return serviceProvider.CreateScope();
    }

    private static IServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddDbContext<PostgresContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<PanierExpirationService>();
        return services.BuildServiceProvider();
    }
}

