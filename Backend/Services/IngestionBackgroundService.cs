using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class IngestionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IngestionBackgroundService> _logger;

    public IngestionBackgroundService(IServiceProvider serviceProvider, ILogger<IngestionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// This method runs automatically as soon as the 'dotnet run' command starts the host.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Ingestion Service is warming up...");

        // 1. Wait a few seconds to ensure Docker containers (Postgres & Ollama) are fully initialized
        // This prevents "Connection Refused" errors on cold starts.
        await Task.Delay(7000, stoppingToken); 

        _logger.LogInformation("Starting automatic ingestion check...");

        try
        {
            // 2. Create a Scope. 
            // We MUST do this because IngestionService and Repository are 'Scoped' 
            // (they live and die with a database connection).
            using (var scope = _serviceProvider.CreateScope())
            {
                var ingestionService = scope.ServiceProvider.GetRequiredService<IStoryIngestionService>();

                // 3. Run the ingestion logic
                await ingestionService.RunFullIngestionAsync();
                
                _logger.LogInformation("Automatic Ingestion completed successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the automatic startup ingestion.");
        }

        _logger.LogInformation("Background Ingestion Service is now idle.");
    }
}