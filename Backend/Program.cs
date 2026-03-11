using Backend.Services;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Added OpenAPI/Swagger for testing endpoints
builder.Services.AddOpenApi();

// Added controllers so the app can find our controllers
builder.Services.AddControllers();

// Register IngestionService so it can be injected into controllers and other services
builder.Services.AddScoped<IngestionService>();

// Register AppDbContext with PostgreSQL and pgvector
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));

// Register OllamaService so it can be used throughout the app
builder.Services.AddScoped<IOllamaService, OllamaService>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();

var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controller routes automatically
app.MapControllers();

app.Run();