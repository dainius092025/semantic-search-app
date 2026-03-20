using Backend.Services;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Added OpenAPI/Swagger for testing endpoints
builder.Services.AddOpenApi();

// Added controllers so the app can find our controllers
builder.Services.AddControllers();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register IngestionService so it can be injected into controllers and other services
builder.Services.AddScoped<IStoryIngestionService, IngestionService>();

// Register AppDbContext with PostgreSQL and pgvector
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));


// Register OllamaService so it can be used throughout the app
builder.Services.AddScoped<IOllamaService, OllamaService>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();

//this bit tells ASP.NET dependancy injection(when someone asks for ISearchServic, create and privide a SearcHService)
builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

// Apply pending EF Core migrations automatically on startup, so we do not need to type `dotnet ef database update` we start from scratch
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// Map controller routes automatically
app.MapControllers();

app.Run();