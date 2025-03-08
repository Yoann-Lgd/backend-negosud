using backend_negosud.Services;

namespace backend_negosud.Extentions;

public class StartupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PanierExpirationService> _loggerPanier;
    private readonly ILogger<ReaproService> _loggerStocks;

    public StartupService(
        IServiceProvider serviceProvider, 
        ILogger<PanierExpirationService> loggerPanier,
        ILogger<ReaproService> loggerStocks)
    {
        _serviceProvider = serviceProvider;
        _loggerPanier = loggerPanier;
        _loggerStocks = loggerStocks;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        PanierExpirationService.Initialize(_serviceProvider, _loggerPanier);
        ReaproService.Initialize(_serviceProvider, _loggerStocks);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}