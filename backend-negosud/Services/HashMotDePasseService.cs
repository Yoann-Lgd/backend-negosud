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
        var randomNumberA = new byte[4];
        var randomNumberB = new byte[4];
        RandomNumberGenerator.Fill(randomNumberA);
        RandomNumberGenerator.Fill(randomNumberB);
        var intA = BitConverter.ToInt32(randomNumberA, 0);
        var intB = BitConverter.ToInt32(randomNumberB, 0);
        var sqids = new SqidsEncoder<int>();
        var MotDePasseTemporaire = sqids.Encode(intA, intB);
        return MotDePasseTemporaire;
    }
}