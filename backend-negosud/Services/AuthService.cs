using backend_negosud.Entities;
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
    var utilisateur = await _utilisateurService.GetUtilisateuByEmail(email);
    if (utilisateur == null)
    {
        return new ResponseDataModel<string>
        {
            Message = "Utilisateur non trouvé",
            Success = false
        };
    }
    
    var derniereDemande = await _context.ReinitialisationMdps
        .Where(r => r.UtilisateurId == utilisateur.UtilisateurId)
        .OrderByDescending(r => r.DateDemande)
        .FirstOrDefaultAsync();

    if (derniereDemande != null && derniereDemande.DateDemande.AddMinutes(5) > DateTime.Now)
    {
        return new ResponseDataModel<string>
        {
            Message = "Email de reset envoyé, réessayez dans 5min",
            Success = false
        };
    }

    var motDePasseTemporaire = _hash.RandomMotDePasseTemporaire();
    var motDePasseHash = _hash.HashMotDePasse(motDePasseTemporaire);
    
    utilisateur.MotDePasse = motDePasseHash;
    
    var nouvelleDemande = new ReinitialisationMdp
    {
        UtilisateurId = utilisateur.UtilisateurId,
        DateDemande = DateTime.Now,
        MotDePasse = motDePasseHash  // Stocker le hash, pas le mot de passe en clair
    };

    _context.ReinitialisationMdps.Add(nouvelleDemande);
    await _context.SaveChangesAsync();

    var emailSubject = "Réinitialisation du mot de passe";
    var emailBody = $@"
                <h2>Mot de passe réinitialisé</h2>
                <p>Votre mot de passe a été réinitialisé avec succès. Voici votre nouveau mot de passe temporaire :</p>
                <p><strong>{motDePasseTemporaire}</strong></p>
                <p>Pour des raisons de sécurité, veuillez changer ce mot de passe dès votre prochaine connexion.</p>
                <p>Cordialement,<br>L'équipe de NegoSud</p>";

    await _emailService.SendEmailAsync(email, emailSubject, emailBody, isHtml: true);
    
    return new ResponseDataModel<string>
    {
        Message = "Un nouveau mot de passe vous a été envoyé par email",
        Success = true
    };
}
}