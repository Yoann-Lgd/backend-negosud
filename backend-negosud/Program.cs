using System.Security.Cryptography;
using backend_negosud.Endpoints;
using backend_negosud.Entities;
using backend_negosud.Extentions;
using backend_negosud.Repository;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using backend_negosud.Seeds;
using FluentValidation;
using FluentValidation.AspNetCore;
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


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IHashMotDePasseService, HashMotDePasseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnvoieEmailService, EnvoieEmailService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration.AddEnvironmentVariables();
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


builder.InjectDependencies();


// Configuration de Serilog
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Hour);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);



var app = builder.Build();

// On peuple la base de données à l'aide de notre classe dédiée et alimnentée par le package Bogus
using (var scope = app.Services.CreateScope())
{
    // var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PostgresContext>();
    var seedData = new DatabaseSeeder(context);
    seedData.SeedDatabase();
}

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

app.MapGroup("/auth").MapAuthEndpoints();

app.Run();
