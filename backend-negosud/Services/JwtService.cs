using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using backend_negosud.entities;
using Microsoft.IdentityModel.Tokens;

namespace backend_negosud.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private RSA LoadOrCreateRsaKey(string path)
    {
        if (File.Exists(path))
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(File.ReadAllBytes(path), out _);
            return rsa;
        }
        else
        {
            var rsa = RSA.Create(2048);
            File.WriteAllBytes(path, rsa.ExportRSAPrivateKey());
            return rsa;
        }
    }
    public string GenererToken(Utilisateur utilisateur)
    {
        RSA rsa = LoadOrCreateRsaKey("key.bin");
        var key = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, utilisateur.UtilisateurId.ToString()),
            new Claim(ClaimTypes.Email, utilisateur.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        RSA rsa = LoadOrCreateRsaKey("key.bin");
        var key = new RsaSecurityKey(rsa);
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero, 
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }
        catch
        {
            return false;
        }
        return true;
    }
}