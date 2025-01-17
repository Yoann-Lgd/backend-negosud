using System.Security.Cryptography;
using backend_negosud.Endpoints;
using backend_negosud.entities;
using backend_negosud.Repository;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
RSA rsa = RSA.Create();
const string keyFileName = "key.bin";
if (!File.Exists(keyFileName))
{
    var key = rsa.ExportRSAPrivateKey();
    File.WriteAllBytes(keyFileName, key);
}
else
{
    rsa.ImportRSAPrivateKey(File.ReadAllBytes(keyFileName), out _);
}
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer((options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    }));

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IHashMotDePasseService, HashMotDePasseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnvoieEmailService, EnvoieEmailService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Token JWT. Saisir \"Bearer {Token}\""
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
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
