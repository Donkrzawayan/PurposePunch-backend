using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Infrastructure.Jobs;

public class DecisionExpirationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DecisionExpirationJob> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public DecisionExpirationJob(IServiceScopeFactory scopeFactory, ILogger<DecisionExpirationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Decision Expiration Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredDecisionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking decisions as abandoned.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredDecisionsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IDecisionRepository>();
        var cutoff = DateTime.UtcNow;

        var count = await repo.MarkExpiredAsAbandonedAsync(cutoff);

        if (count > 0)
            _logger.LogInformation("Marked {Count} decisions as Abandoned.", count);
    }
}
