using Backend.Services;
using Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Added OpenAPI/Swagger for testing endpoints
builder.Services.AddOpenApi();

// Added controllers so the app can find our controllers
builder.Services.AddControllers();

// Register OllamaService so it can be used throughout the app
builder.Services.AddScoped<IOllamaService, OllamaService>();

var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controller routes automatically
app.MapControllers();

app.Run();