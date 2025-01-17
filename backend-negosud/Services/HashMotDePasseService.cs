using System.Security.Cryptography;
using Sqids;

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

    public string RandomMotDePasseTemporaire()
    {
        Random rnd = new Random();
        int intA  = rnd.Next(1, 13);
        int intB   = rnd.Next(1, 7);
        var sqids = new SqidsEncoder<int>();
        var MotDePasseTemporaire = sqids.Encode(intA, intB);
        return MotDePasseTemporaire;
    }
}