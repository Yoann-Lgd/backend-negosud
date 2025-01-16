namespace backend_negosud.Services;

public class HashMotDePasseService : IHashMotDePasseService
{
    public string HashMotDePasse(string motDePasse)
    {
        return BCrypt.Net.BCrypt.HashPassword(motDePasse, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerifyMotDePasse(string motDePasse, string hashedmotDePasse)
    {
        return BCrypt.Net.BCrypt.Verify(motDePasse, hashedmotDePasse);
    }
}