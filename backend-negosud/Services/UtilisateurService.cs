using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public class UtilisateurService : IUtilisateurService
{
    
    private readonly PostgresContext _context;
    private readonly IUtilisateurRepository _repository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly ILogger<UtilisateurService> _logger;

    public UtilisateurService(IUtilisateurRepository utilisateurRepository, PostgresContext context, IMapper mapper, IJwtService jwtService, IHashMotDePasseService hash, ILogger<UtilisateurService> logger)
    {
        _repository = utilisateurRepository;
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
        _hash = hash;
        _logger = logger;
    }

        public async Task<IResponseDataModel<UtilisateurOutputDto>> CreateUtilisateur(UtilisateurInputDto utilisateurInputDto)
    {
        try
        {
            // Validation des entrées
            if (string.IsNullOrEmpty(utilisateurInputDto.Email) || string.IsNullOrEmpty(utilisateurInputDto.MotDePasse))
            {
                _logger.LogWarning("Tentative de création d'utilisateur avec email ou mot de passe vide");
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email ou mot de passe non spécifié"
                };
            }

            // Vérification email existant
            if (await _repository.EmailExistsAsync(utilisateurInputDto.Email))
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }

            // Validation
            var validation = new UtilisateurValidation();
            var result = validation.Validate(utilisateurInputDto);

            if (!result.IsValid)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Données utilisateur invalides: " + string.Join(", ", result.Errors)
                };
            }

            // Création utilisateur
            var utilisateur = _mapper.Map<Utilisateur>(utilisateurInputDto);
            utilisateur.MotDePasse = _hash.HashMotDePasse(utilisateur.MotDePasse);

            // Sauvegarde via repository
            await _repository.AddAsync(utilisateur);

            // Génération token
            var tokenJWT = _jwtService.GenererToken(utilisateur);
            utilisateur.AccessToken = tokenJWT;

            await _repository.UpdateAsync(utilisateur);

            var userOutput = _mapper.Map<UtilisateurOutputDto>(utilisateur);

            return new ResponseDataModel<UtilisateurOutputDto>
            {
                TokenJWT = tokenJWT,
                Success = true,
                Data = userOutput
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création de l'utilisateur");
            throw;
        }
    }

    public async Task<Utilisateur> GetUtilisateurByToken(string token)
    {
        return await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.AccessToken.ToString() == token);
    }

    public async Task UpdateUtilisateur(Utilisateur utilisateur)
    {
        _context.Utilisateurs.Update(utilisateur);
        await _context.SaveChangesAsync();
    }

    public async Task<Utilisateur> GetUtilisateuByEmail(string email)
    {
        return await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);
    }
}