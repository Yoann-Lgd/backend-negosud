using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Services;
using backend_negosud.Validation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly ILogger<UtilisateurRepository> _logger;

    public UtilisateurRepository(
        PostgresContext context, 
        IMapper mapper, 
        IJwtService jwtService, 
        IHashMotDePasseService hash,
        ILogger<UtilisateurRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /*public async Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto utilisateurInputDto)
    {
       
    }*/

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Utilisateurs
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
        /*try
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
            
            // Vérification de l'unicité de l'email
            var mailExistant = await _context.Utilisateurs
                .AnyAsync(u => u.Email.ToLower() == utilisateurInputDto.Email.ToLower());
                
            if (mailExistant)
            {
                _logger.LogWarning("Tentative de création d'un compte avec un email déjà existant: {Email}", utilisateurInputDto.Email);
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }
            
            // Utilisation du validator pou verifier le format des données rentrées
            UtilisateurValidation validation = new UtilisateurValidation();
            ValidationResult result = validation.Validate(utilisateurInputDto);

            if (!result.IsValid) 
            {
                _logger.LogWarning("Validation échouée pour la création d'utilisateur: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Données utilisateur invalides: " + string.Join(", ", result.Errors.Select(e => e.ErrorMessage))
                };
            }

            // Création de l'utilisateur
            var utilisateur = _mapper.Map<Utilisateur>(utilisateurInputDto);
            utilisateur.MotDePasse = _hash.HashMotDePasse(utilisateur.MotDePasse);
            
            // Sauvegarde initiale pour obtenir l'ID
            await _context.Utilisateurs.AddAsync(utilisateur);
            await _context.SaveChangesAsync();

            // Génération du token avec l'ID maintenant disponible
            var tokenJWT = _jwtService.GenererToken(utilisateur);
            utilisateur.AccessToken = tokenJWT;
            
            // Mise à jour du token dans la base
            await _context.SaveChangesAsync();

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
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la création de l'utilisateur"
            };
        }*/
        public async Task<ResponseDataModel<UtilisateurOutputDto>> AddAsync(Utilisateur entity,
            CancellationToken cancellationToken = default)
        {
            try 
            {
                var utilisateur = _mapper.Map<Utilisateur>(entity);
                await _context.Utilisateurs.AddAsync(utilisateur);
                await _context.SaveChangesAsync();
                var userOutput = _mapper.Map<UtilisateurOutputDto>(utilisateur);
                return new ResponseDataModel<UtilisateurOutputDto>
                {
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

        public Task<ResponseDataModel<UtilisateurOutputDto>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDataModel<List<UtilisateurOutputDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Utilisateur entity, CancellationToken cancellationToken = default)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(Utilisateur entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
}
