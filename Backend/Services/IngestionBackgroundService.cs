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

        try
        {
            // 1. Create a Scope to get the Ollama service
            using (var scope = _serviceProvider.CreateScope())
            {
                var ollamaService = scope.ServiceProvider.GetRequiredService<IOllamaService>();
                
                // 2. Wait for Ollama and models to be ready
                bool ready = await ollamaService.WaitForModelsAsync(stoppingToken);
                
                if (!ready)
                {
                    _logger.LogWarning("Ollama was not ready in time. Skipping automatic ingestion.");
                    return;
                }

                _logger.LogInformation("Starting automatic ingestion check...");

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