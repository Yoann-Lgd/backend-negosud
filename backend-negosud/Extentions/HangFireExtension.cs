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
            () => PanierExpirationService.CleanupExpiredBasketsAsync(),
            "*/30 * * * *"); // exec toutes les 30 minutes
        
        // tâche récurrente pour reaprovisionner les stocks auto 
        recurringJobManager.AddOrUpdate(
            "reapro-low-stocks",
            () => ReaproService.CheckAndReapprovisionnerAsync(),
            "*/05 * * * *", TimeZoneInfo.Local); // exec toutes les 5 minutes
    }
}