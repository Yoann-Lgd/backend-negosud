using backend_negosud.entities;
using backend_negosud.Models;

namespace backend_negosud.Services;


public class AuthService : IAuthService

{
    private readonly IHashMotDePasseService _hash;
    private readonly IUtilisateurService _utilisateurService;
    private readonly IJwtService _jwtService;

    public AuthService(IJwtService jwtService, IUtilisateurService utilisateurService, IHashMotDePasseService hash)
    {
        _hash = hash;
        _utilisateurService = utilisateurService;
        _jwtService = jwtService;
    }

    public async Task<IResponseDataModel<Utilisateur>> Login(string email, string motDePasse)
    {
        var utilisateur = await _utilisateurService.GetUtilisateuByEmail(email);
        if (utilisateur == null)
        {
            return new ResponseDataModel<Utilisateur>
            {
                Message = "Email ou mot de passe incorrect",
                Success = false
            };
        }

        /*if (!user.MailValide)
        {
            return new LoginResult(false, "Veuillez vérifier votre email avant de vous connecter.");
        }*/

        var tokenJWT = _jwtService.GenererToken(utilisateur);
        utilisateur.AccessToken = tokenJWT;
        return new ResponseDataModel<Utilisateur>
        {
            Success = true,
            Message = "Connexion réussie.",
            TokenJWT = tokenJWT
        };
    }
}