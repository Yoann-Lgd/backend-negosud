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

            RSA rsa = LoadOrCreateRsaKey();
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            // Utilisation de réflexion pour obtenir l'ID et l'email
            var idProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
            
            var emailProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Email", StringComparison.OrdinalIgnoreCase));

            if (idProperty == null || emailProperty == null)
            {
                throw new InvalidOperationException("Impossible de trouver les propriétés ID et Email");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idProperty.GetValue(entity).ToString()),
                new Claim(ClaimTypes.Email, emailProperty.GetValue(entity).ToString()),
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
    
    public async Task<string> GetPublicKey()
    {
        using var rsa = LoadOrCreateRsaKey();
        return await Task.FromResult(Convert.ToBase64String(rsa.ExportRSAPublicKey()));
    }
}