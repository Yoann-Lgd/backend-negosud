using backend_negosud.DTOs;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
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
        endpoints.MapPost("/login",
            async ([FromServices] IAuthService auth, [FromBody] UtilisateurInputDto utilisateurInputDto) =>
            {
                var result = await auth.Login(utilisateurInputDto.Email, utilisateurInputDto.MotDePasse);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });
        endpoints.MapPost("/resetmotdepasse",
            async ([FromServices] IAuthService auth, [FromBody] UtilisateurInputDto utilisateurInputDto) =>
            {
                var result = await auth.ResetMotDePasse(utilisateurInputDto.Email);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });
        
        endpoints.MapGet("/protectedTest", [Authorize] async (HttpContext context) =>
        {
            var result = "Hello autorized";
            return Results.Ok(result);
        });
        return endpoints;
    }
}