using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using backend_negosud.Entities;
using backend_negosud.Mapper;
using backend_negosud.Services;
using Microsoft.IdentityModel.Tokens;

public class JwtService<TEntity, TInputDto, TOutputDto> : IJwtService<TEntity, TInputDto, TOutputDto>
    where TEntity : class
    where TInputDto : class
    where TOutputDto : class
{
    private readonly IConfiguration _configuration;
    private readonly PostgresContext _context;
    private readonly  IMapper _mapper;
    private readonly string _keyPath;
    private readonly ILogger<JwtService<TEntity, TInputDto, TOutputDto>> _logger;

    public JwtService(
        IConfiguration configuration, 
        PostgresContext context,
        IMapper mapper,
        ILogger<JwtService<TEntity, TInputDto, TOutputDto>> logger)
    {
        _configuration = configuration;
        _context = context;
        _mapper = mapper;
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

    public string GenererToken(TInputDto inputDto)
    {
        try
        {
            var entity = _mapper.Map<TEntity>(inputDto);

            // Générer la clé RSA
            RSA rsa = LoadOrCreateRsaKey();
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            // récupération les propriétés de l'entité
            var idProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            var emailProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Email", StringComparison.OrdinalIgnoreCase));

            var roleProperty = typeof(TEntity).GetProperty("Role");

            if (idProperty == null || emailProperty == null)
            {
                throw new InvalidOperationException("Impossible de trouver les propriétés ID et Email");
            }

            // vérif et récupération des valeurs
            var idValue = idProperty.GetValue(entity)?.ToString();
            var emailValue = emailProperty.GetValue(entity)?.ToString();
            var roleValue = roleProperty?.GetValue(entity) is Role role ? role.Nom : null;

            if (string.IsNullOrEmpty(idValue) || string.IsNullOrEmpty(emailValue))
            {
                throw new InvalidOperationException("Les valeurs ID ou Email sont nulles.");
            }

            // création des claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, idValue),
                new Claim(ClaimTypes.Email, emailValue),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            // ajout du rôle s'il est valide
            if (!string.IsNullOrEmpty(roleValue))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleValue));
            }

            // déf de la durée d'expiration du token
            var tokenExpiration = DateTime.UtcNow.AddHours(
                double.Parse(_configuration["Jwt:ExpirationHours"] ?? "6")
            );

            // génération du token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated Token: {tokenString}");

            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du token");
            throw;
        }
    }

    public async Task<TOutputDto> ValidateToken(string token, int id)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var entity = await _context.FindAsync<TEntity>(id);

            if (entity == null)
            {
                return null;
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

            var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdClaim == null || !int.TryParse(nameIdClaim.Value, out var tokenUserId) || tokenUserId != id)
            {
                return null;
            }
            
            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            if (roleClaim != null)
            {
                // vérif si nécessaire
            }

            // Mettre à jour le token d'accès si nécessaire
            var accessTokenProperty = entity.GetType().GetProperty("AccessToken");
            if (accessTokenProperty != null)
            {
                accessTokenProperty.SetValue(entity, token);
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<TOutputDto>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du token pour l'ID {Id}", id);
            return null;
        }
    }

}