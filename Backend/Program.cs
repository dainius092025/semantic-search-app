using Backend.Services;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Added OpenAPI/Swagger for testing endpoints
builder.Services.AddOpenApi();

// Added controllers so the app can find our controllers
builder.Services.AddControllers();

// Register AppDbContext with PostgreSQL and pgvector
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));

// Register OllamaService so it can be used throughout the app
builder.Services.AddScoped<IOllamaService, OllamaService>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IngestionService>();

var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controller routes automatically
app.MapControllers();

// Automatically run ingestion at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var ingestionService = scope.ServiceProvider.GetRequiredService<IngestionService>();

    try 
    {
        // Ensure database schema is created and up-to-date
        Console.WriteLine("Ensuring database is migrated...");
        await context.Database.MigrateAsync();

        Console.WriteLine("Starting automatic data ingestion...");
        await ingestionService.RunFullIngestionAsync();
        Console.WriteLine("Automatic data ingestion completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Startup initialization failed: {ex.Message}");
    }
}

app.Run();

