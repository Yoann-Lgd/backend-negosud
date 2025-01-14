using backend_negosud.Endpoints;
using backend_negosud.entities;
using backend_negosud.Repository;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Passeports Backend",
        Version = "v1"
    });
});

// Configuration de Serilog
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Hour);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

// Configuration de la connexion à la base de données
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PostgresContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Test de connexion à la base de données
try
{
    using (var connection = new NpgsqlConnection(connectionString))
    {
        connection.Open();
        logger.Information("Connexion réussie à la base de données PostgreSQL.");
    }
}
catch (Exception ex)
{
    logger.Error(ex, "Erreur lors de la connexion à la base de données");
}

app.MapGroup("/auth").MapAuthEndpoints();

app.Run();
