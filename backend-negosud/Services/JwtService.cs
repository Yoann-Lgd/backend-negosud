using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly PostgresContext _context;
    private readonly string _keyPath;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IConfiguration configuration, 
        PostgresContext context,
        ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _context = context;
        _logger = logger;
        _keyPath = _configuration["Jwt:KeyPath"] ?? "key.bin";
    }

    private RSA GenerateRsaKey()
    {
        try
        {
            return RSA.Create(2048);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération de la clé RSA");
            throw;
        }
    }

    private RSA LoadOrCreateRsaKey()
    {
        try
        {
            if (File.Exists(_keyPath))
            {
                RSA rsaC = RSA.Create();
                byte[] keyBytes = File.ReadAllBytes(_keyPath);
                rsaC.ImportRSAPrivateKey(keyBytes, out _);
                return rsaC;
            }
            
            RSA rsa = GenerateRsaKey();
            Directory.CreateDirectory(Path.GetDirectoryName(_keyPath));
            File.WriteAllBytes(_keyPath, rsa.ExportRSAPrivateKey());
            return rsa;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement/création de la clé RSA");
            throw;
        }
    }

    public string GenererToken(Utilisateur utilisateur)
    {
        try
        {
            using var rsa = LoadOrCreateRsaKey();
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.UtilisateurId.ToString()),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            var tokenExpiration = DateTime.UtcNow.AddHours(
                double.Parse(_configuration["Jwt:ExpirationHours"] ?? "6")
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du token pour l'utilisateur {UserId}", utilisateur.UtilisateurId);
            throw;
        }
    }

    public async Task<bool> ValidateToken(string token, int id)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            
            if (utilisateur == null)
            {
                return false;
            }

            using var rsa = LoadOrCreateRsaKey();
            var key = new RsaSecurityKey(rsa);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Vérification supplémentaire que l'ID dans le token correspond
            var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdClaim == null || !int.TryParse(nameIdClaim.Value, out var tokenUserId) || tokenUserId != id)
            {
                return false;
            }

            utilisateur.AccessToken = token;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du token pour l'utilisateur {UserId}", id);
            return false;
        }
    }
}