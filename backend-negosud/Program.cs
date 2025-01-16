using backend_negosud.Endpoints;
using backend_negosud.entities;
using backend_negosud.Extentions;
using backend_negosud.Repository;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend_negosud.Seeds;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


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
