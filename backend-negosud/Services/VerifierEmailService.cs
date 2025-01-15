using backend_negosud.Models;

namespace backend_negosud.Services;

public class VerifierEmailService : IVerifierEmailService
{
    private readonly IUtilisateurService _utilisateurService;

    public VerifierEmailService(IUtilisateurService utilisateurService)
    {
        _utilisateurService = utilisateurService;
    }

    public async Task<ResponseModel> Verifier(Guid token)
    {
        var user = await _utilisateurService.GetUtilisateurByToken(token.ToString());
        if (user == null)
        {
            return new ResponseModel
            {
                Success = false,
                Message = "Utilisateur non trouvé"
            };
        }

        if (user.MailValide)
        {
            return new ResponseModel
            {
                Success = false,
                Message = "L'email a déjà été vérifié."
            };
        }

        user.MailValide = true;
        user.AccessToken = null;

        await _utilisateurService.UpdateUtilisateur(user);

        return new ResponseModel
        {
            Success = true,
            Message = "Email vérifié avec succès."
        };
    }
}