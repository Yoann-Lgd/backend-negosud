using backend_negosud.entities;
using backend_negosud.Repository;
using backend_negosud.Seeds;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace backend_negosud.Extentions;

public static class DependencyInjectionExtension
{

    public static void InjectDependencies(this WebApplicationBuilder builder)
    {
        builder.AddRepositories();
        builder.AddServices();
        builder.Services.AddControllers();
        builder.AddSeeder();
        builder.AddEFCoreConfiguration();
        builder.CorseConfiguration();
        builder.AddSwagger();
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
    }    
    
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();
    }    
    
    public static void AddSeeder(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DatabaseSeeder>();
    }

    public static void AddEFCoreConfiguration(this WebApplicationBuilder builder)
    {
        // Configuration de la connexion à la base de données
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<PostgresContext>(options => options.UseNpgsql(connectionString));
    }
    
    
    public static void CorseConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        }); 
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        // Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Passeports Backend",
                Version = "v1"
            });
        });
    }
}