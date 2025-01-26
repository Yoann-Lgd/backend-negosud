using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly PostgresContext _context;
    private readonly IUtilisateurService _utilisateurService;
    private readonly IJwtService _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly IEnvoieEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IJwtService jwtService, 
        IUtilisateurService utilisateurService, 
        PostgresContext context, 
        IHashMotDePasseService hash, 
        IEnvoieEmailService emailService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _context = context;
        _utilisateurService = utilisateurService;
        _jwtService = jwtService;
        _hash = hash;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResponseDataModel<UtilisateurOutputDto>> Login(string email, string motDePasse)
    {
        try 
        {
            var utilisateur = await _utilisateurService.GetUtilisateuByEmail(email);
            if (utilisateur == null)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Message = "Email ou mot de passe incorrect",
                    Success = false
                };
            }

            if (!_hash.VerifyMotDePasse(motDePasse, utilisateur.MotDePasse))
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Mot de passe incorrect"
                };
            }
            
            var tokenJWT = _jwtService.GenererToken(utilisateur);
            utilisateur.AccessToken = tokenJWT;
            
            var outputDto = _mapper.Map<UtilisateurOutputDto>(utilisateur);

            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = true,
                Message = "Connexion réussie.",
                TokenJWT = tokenJWT,
                Data = outputDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion");
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la connexion"
            };
        }
    }

    public async Task<IResponseDataModel<string>> ResetMotDePasse(string email)
    {
        try 
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
                MotDePasse = motDePasseHash
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la réinitialisation du mot de passe"
            };
        }
    }
}