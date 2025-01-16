using backend_negosud.entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;


public class AuthService : IAuthService

{
    private readonly PostgresContext _context;
    private readonly IUtilisateurService _utilisateurService;
    private readonly IJwtService _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly IEnvoieEmailService _emailService;

    public AuthService(IJwtService jwtService, IUtilisateurService utilisateurService, PostgresContext context, IHashMotDePasseService hash, IEnvoieEmailService emailService)
    {
        _context = context;
        _utilisateurService = utilisateurService;
        _jwtService = jwtService;
        _hash = hash;
        _emailService = emailService;
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
        if (!_hash.VerifyMotDePasse(motDePasse, utilisateur.MotDePasse))
        {
            return new ResponseDataModel<Utilisateur>
            {
                Success = false,
                Message = "Mot de passe incorrecte",
  
            };
        }
        
        var tokenJWT = _jwtService.GenererToken(utilisateur);
        utilisateur.AccessToken = tokenJWT;
        return new ResponseDataModel<Utilisateur>
        {
            Success = true,
            Message = "Connexion réussie.",
            TokenJWT = tokenJWT
        };
       
    }

    public async Task<IResponseDataModel<string>> ResetMotDePasse(string email)
    {
      
      var NouveauMotDePasse = _hash.RandomMotDePasseTemporaire();
      var utilisateur = await _utilisateurService.GetUtilisateuByEmail(email);
      utilisateur.MotDePasse = NouveauMotDePasse;
      await _context.SaveChangesAsync();
      var emailSubject = "Réinitialisation du mot de passe";
      var emailBody = $@"
                    <h2>Mot de passe réinitialisé</h2>
                    <p>Votre mot de passe a été réinitialisé avec succès. Voici votre nouveau mot de passe :</p>
                    <p><strong>{NouveauMotDePasse}</strong></p>
                    <p>Vous pouvez vous connecter à l'application en utilisant ce mot de passe. Nous vous recommandons de le changer dès que possible.</p>
                    <p>Cordialement,<br>L'équipe de NegoSud</p>";

      await _emailService.SendEmailAsync(email, emailSubject, emailBody, isHtml: true);
      return new ResponseDataModel<string>
      {
          Message = "Nouveau mot de passe:" + NouveauMotDePasse,
          Success = true
      };

    }
}