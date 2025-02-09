using backend_negosud.Services;

namespace backend_negosud.Extentions;

public class StartupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PanierExpirationService> _logger;

    public StartupService(IServiceProvider serviceProvider, ILogger<PanierExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        PanierExpirationService.Initialize(_serviceProvider, _logger);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}