using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Repository;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Seeds;
using backend_negosud.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Stripe;

namespace backend_negosud.Extentions;

public static class DependencyInjectionExtension
{

    public static void InjectDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
        builder.AddRepositories();
        builder.AddServices();
        builder.Services.AddControllers();
        builder.AddSeeder();
        builder.AddEFCoreConfiguration();
        builder.CorseConfiguration(); 
        builder.AddSwagger();
        builder.AddStripeConfiguration();
        builder.AddHangfireConfiguration();
        // builder.AddReferenceHandler();
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
        builder.Services.AddScoped<IFournisseurRepository, FournisseurRepository>();
        builder.Services.AddScoped<IFamilleRepository, FamilleRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IStockRepository, StockRepository>();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IBonCommandeRepository, BonCommandeRepository>();
        builder.Services.AddScoped<IInventorierRepository, InventorierRepository>();
        builder.Services.AddScoped<ILigneBonCommandeRepository, LigneBonCommandeRepository>();
        builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();
        builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
        builder.Services.AddScoped<IPaysRepository, PaysRepository>();
        builder.Services.AddScoped<IFactureRepository, FactureRepository>();
        builder.Services.AddScoped<IReglementRepository, ReglementRepository>();
    }
    
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();
        builder.Services.AddScoped<IFournisseurService, FournisseurService>();
        builder.Services.AddScoped<IArticleService, ArticleService>();
        builder.Services.AddScoped<StripeService>();
        builder.Services.AddScoped<IStockService, StockService>();
        builder.Services.AddScoped<ICommandeService, CommandeService>();
        builder.Services.AddScoped<IBonCommandeService, BonCommandeService>();
        builder.Services.AddScoped<IPanierService, PanierService>();        
        builder.Services.AddScoped<IRoleService, RoleService>();        
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IPaysService, PaysService>();
        builder.Services.AddScoped<IJwtService<Client, ClientInputDto, ClientOutputDto>, JwtService<Client, ClientInputDto, ClientOutputDto>>();
        builder.Services.AddScoped<IAuthService<Client, ClientInputDto, ClientOutputDto>, ClientService>();
        builder.Services.AddScoped<IJwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto>, JwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto>>();
        builder.Services.AddScoped<IAuthService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto>, UtilisateurService>();
        builder.Services.AddScoped<IHashMotDePasseService, HashMotDePasseService>();
        builder.Services.AddScoped<IEnvoieEmailService, EnvoieEmailService>();
        builder.Services.AddScoped<IFamilleService, FamilleService>();
        builder.Services.AddScoped<ILogger<Program>, Logger<Program>>();
        builder.Services.AddHostedService<StartupService>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddFluentValidationAutoValidation();
    }    
    public static void AddHangfireConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
            config.UseSerializerSettings(new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        });
        builder.Services.AddHangfireServer();
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

    public static void AddStripeConfiguration(this WebApplicationBuilder builder)
    {
        var stripeSettings = builder.Configuration.GetSection("Stripe");
        StripeConfiguration.ApiKey = stripeSettings["SecretKey"];
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

    // public static void AddReferenceHandler(this WebApplicationBuilder builder)
    // {
    //     builder.Services.AddControllers().AddJsonOptions(options =>
    //     {
    //         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    //         options.JsonSerializerOptions.WriteIndented = true;
    //     });
    //
    // }

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