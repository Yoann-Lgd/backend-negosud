using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.DTOs.Utilisateur.Input;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public class UtilisateurService : IUtilisateurService
{
    
    private readonly PostgresContext _context;
    private readonly IUtilisateurRepository _repository;
    private readonly IRoleRepository _role_repository;
    private readonly IMapper _mapper;
    private readonly IJwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto> _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly ILogger<UtilisateurService> _logger;
    private readonly IEnvoieEmailService _emailService;

    public UtilisateurService(IEnvoieEmailService emailService, IUtilisateurRepository utilisateurRepository,  IRoleRepository roleRepository, PostgresContext context, IMapper mapper, IJwtService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto> jwtService, IHashMotDePasseService hash, ILogger<UtilisateurService> logger)
    {
        _repository = utilisateurRepository;
        _role_repository = roleRepository;
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
        _emailService = emailService;
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
                
                var role = await _role_repository.GetByIdAsync(utilisateurInputDto.RoleId); 

                if (role == null)
                {
                    return new ResponseDataModel<UtilisateurOutputDto>
                    {
                        Success = false,
                        Message = "Le rôle spécifié n'existe pas."
                    };
                }

                // Création utilisateur
                var utilisateur = _mapper.Map<Utilisateur>(utilisateurInputDto);
                utilisateur.MotDePasse = _hash.HashMotDePasse(utilisateur.MotDePasse);
                utilisateur.Role = role;

                // Sauvegarde via repository
                await _repository.AddAsync(utilisateur);
                
                // Génération token
                var tokenJWT = _jwtService.GenererToken(_mapper.Map<UtilisateurInputDto>(utilisateur));
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

    public async Task<IResponseDataModel<string>> UpdateUtilisateur(int id, UtilisateurInputDtoWithRole utilisateurInputDto)
    {
        try
        {
            var utilisateur = await _repository.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Utilisateur non trouvé.",
                };
            }
            
            if (!string.IsNullOrEmpty(utilisateurInputDto.Prenom))
            {
                utilisateur.Prenom = utilisateurInputDto.Prenom;
            }

            if (!string.IsNullOrEmpty(utilisateurInputDto.Nom))
            {
                utilisateur.Nom = utilisateurInputDto.Nom;
            }

            if (id > 0)
            {
                utilisateur.UtilisateurId = id;
            }

            if (utilisateur.RoleId != 0)
            {
                utilisateur.RoleId = utilisateurInputDto.roleId;
            }
            if (!string.IsNullOrEmpty(utilisateurInputDto.Email))
            {
                utilisateur.Email = utilisateurInputDto.Email;
            }


            await _repository.UpdateAsync(utilisateur);

            return new ResponseDataModel<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "L'article a été mis à jour avec succès.",
                Data = utilisateur.UtilisateurId.ToString(),
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
    
    public async Task<BooleanResponseDataModel> UtilisateurExistEmail(UtilisateurEmailInputDto utilisateurEmailInputDto)
    {
        try
        {
            var validator = new UtilisateurEmailInputDtoValidator();
            
            ValidationResult validationResult = validator.Validate(utilisateurEmailInputDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                return new BooleanResponseDataModel
                {
                    Success = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors),
                    Data = false,
                };
            }

            var emailLower = utilisateurEmailInputDto.Email.ToLower();

            var exists = await _context.Utilisateurs
                .AnyAsync(u => u.Email.ToLower() == emailLower);

            if (exists)
            {
                return new BooleanResponseDataModel
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Un utilisateur avec cet email existe.",
                    Data = true,
                };
            }
            else
            {
                return new BooleanResponseDataModel
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Aucun utilisateur trouvé avec cet email.",
                    Data = false,
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la vérification de l'utilisateur par email.");
            return new BooleanResponseDataModel
            {
                Success = false,
                StatusCode = 500,
                Message = "Une erreur s'est produite lors de la vérification de l'utilisateur.",
                Data = false,
            };
        }
    }


    public async Task<IResponseDataModel<UtilisateurOutputDto>> Login(string email, string motDePasse)
    {
        try 
        {
            var utilisateur = await GetUtilisateuByEmail(email);
            if (utilisateur == null)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Message = "Email ou mot de passe incorrect",
                    Success = false
                };
            }

            if (!_hash.VerifyMotDePasse(motDePasse, utilisateur.MotDePasse))
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Mot de passe incorrect"
                };
            }

            var role = await _role_repository.GetByIdAsync(utilisateur.RoleId);

            if (role == null)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Le rôle spécifié n'existe pas."
                };
            }

            // mapper l'entité en DTO pour la génération du token
            var utilisateurInputDto = _mapper.Map<UtilisateurInputDto>(utilisateur);
            utilisateurInputDto.Role = role?.Nom ?? string.Empty;
            
            var tokenJWT = _jwtService.GenererToken(utilisateurInputDto);
            utilisateur.AccessToken = tokenJWT;

            var outputDto = _mapper.Map<UtilisateurOutputDto>(utilisateur);

            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = true,
                Message = "Connexion réussie.",
                TokenJWT = tokenJWT,
                Data = outputDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion");
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la connexion"
            };
        }
    }
    
        public async Task<IResponseDataModel<UtilisateurOutputDto>> GetUtilisateurById(int id)
        {
            var utilisateur = await _repository.GetByIdAsync(id);
            var utilisateurOutputDto = _mapper.Map<UtilisateurOutputDto>(utilisateur);
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = true,
                StatusCode = 200,
                Data = utilisateurOutputDto,
            };
        }

    public async Task<IResponseDataModel<List<UtilisateurOutputDto>>> GetAllUtilisateurs()
        {
            var utilisateurs = await _repository.GetUtilisateursWithRole();
            var output = _mapper.Map<List<UtilisateurOutputDto>>(utilisateurs);
        
            return new ResponseDataModel<List<UtilisateurOutputDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = output,
            };
        }

    public async Task<IResponseDataModel<string>> ResetMotDePasse(string email)
    {
        try 
        {
            var utilisateur = await GetUtilisateuByEmail(email);
            if (utilisateur == null)
            {
                return new ResponseDataModel<string>
                {
                    Message = "Utilisateur non trouvé",
                    Success = false
                };
            }

            var derniereDemande = await _context.ReinitialisationMdps
                .Where(r => r.UtilisateurId == utilisateur.UtilisateurId)
                .OrderByDescending(r => r.DateDemande)
                .FirstOrDefaultAsync();

            if (derniereDemande != null && derniereDemande.DateDemande.AddMinutes(5) > DateTime.Now)
            {
                return new ResponseDataModel<string>
                {
                    Message = "Email de reset envoyé, réessayez dans 5min",
                    Success = false
                };
            }

            var motDePasseTemporaire = _hash.RandomMotDePasseTemporaire();
            var motDePasseHash = _hash.HashMotDePasse(motDePasseTemporaire);
            
            utilisateur.MotDePasse = motDePasseHash;
            
            var nouvelleDemande = new ReinitialisationMdp
            {
                UtilisateurId = utilisateur.UtilisateurId,
                DateDemande = DateTime.Now,
                MotDePasse = motDePasseHash
            };

            _context.ReinitialisationMdps.Add(nouvelleDemande);
            await _context.SaveChangesAsync();

            var emailSubject = "Réinitialisation du mot de passe";
            var emailBody = $@"
                <h2>Mot de passe réinitialisé</h2>
                <p>Votre mot de passe a été réinitialisé avec succès. Voici votre nouveau mot de passe temporaire :</p>
                <p><strong>{motDePasseTemporaire}</strong></p>
                <p>Pour des raisons de sécurité, veuillez changer ce mot de passe dès votre prochaine connexion.</p>
                <p>Cordialement,<br>L'équipe de NegoSud</p>";

            await _emailService.SendEmailAsync(email, emailSubject, emailBody, isHtml: true);
            
            return new ResponseDataModel<string>
            {
                Message = "Un nouveau mot de passe vous a été envoyé par email",
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la réinitialisation du mot de passe"
            };
        }
    }
    public async Task<IResponseDataModel<string>> SoftDeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer l'utilisateur : {Id}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }
            var utilisateur = await _repository.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "L'utilisateur n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }

            var response = await _repository.SoftDeleteEntityByIdAsync<Utilisateur, int>(id);
            if (response)
            {
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Le utilisateur a été soft-supprimé.",
                    StatusCode = 200,
                    Data = id.ToString(),
                };
            }
            else
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le utilisateur n'a pas été trouvé.",
                    StatusCode = 404,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du soft delete de l'utilisateur");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors du soft delete de l'utilisateur.",
                StatusCode = 500,
            };
        }
    }
}