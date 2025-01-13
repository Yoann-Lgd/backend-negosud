using backend_negosud.DTOs;
using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder endpoints)
    {
        endpoints.MapPost("/register", async ([FromServices] IUtilisateurService service, [FromBody] UtilisateurInputDto UtilisateurDto) =>
        {
            var result = await service.CreateUtilisateur(UtilisateurDto);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });
        return endpoints;
    }
}