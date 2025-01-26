using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class ClientService : IClientService
{
    private readonly PostgresContext _context;
    private readonly IClientRepository _repository;
    private readonly IMapper _mapper;
    private readonly IJwtService<Client, ClientInputDto, ClientOutputDto> _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly ILogger<ClientService> _logger;
    private readonly IEnvoieEmailService _emailService;
    
public ClientService(IEnvoieEmailService emailService, IClientRepository ClientRepository, PostgresContext context, IMapper mapper, IJwtService<Client, ClientInputDto, ClientOutputDto> jwtService, IHashMotDePasseService hash, ILogger<ClientService> logger)
    {
        _repository = ClientRepository;
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
        _emailService = emailService;
        _hash = hash;
        _logger = logger;
    }

        public async Task<IResponseDataModel<ClientOutputDto>> CreateClient(ClientInputDto ClientInputDto)
    {
        try
        {
            // Validation des entrées
            if (string.IsNullOrEmpty(ClientInputDto.Email) || string.IsNullOrEmpty(ClientInputDto.MotDePasse))
            {
                _logger.LogWarning("Tentative de création d'Client avec email ou mot de passe vide");
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Email ou mot de passe non spécifié"
                };
            }

            // Vérification email existant
            if (await _repository.EmailExistsAsync(ClientInputDto.Email))
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }

            // Validation
            var validation = new ClientValidation();
            var result = validation.Validate(ClientInputDto);

            if (!result.IsValid)
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Données Client invalides: " + string.Join(", ", result.Errors)
                };
            }

            // Création Client
            var Client = _mapper.Map<Client>(ClientInputDto);
            Client.MotDePasse = _hash.HashMotDePasse(Client.MotDePasse);

            // Sauvegarde via repository
            await _repository.AddAsync(Client);

            // Génération token
            var tokenJWT = _jwtService.GenererToken(_mapper.Map<ClientInputDto>(Client));
            Client.AcessToken = tokenJWT;

            await _repository.UpdateAsync(Client);

            var userOutput = _mapper.Map<ClientOutputDto>(Client);

            return new ResponseDataModel<ClientOutputDto>
            {
                TokenJWT = tokenJWT,
                Success = true,
                Data = userOutput
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création de l'Client");
            throw;
        }
    }

    public async Task<Client> GetClientByToken(string token)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.AcessToken.ToString() == token);
    }

    public async Task UpdateClient(Client Client)
    {
        _context.Clients.Update(Client);
        await _context.SaveChangesAsync();
    }

    public Task<Client> GetClientByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ValidationClientEmail(ClientInputDto clientInputDto)
    {
        // TODO implémenter la logique de validation de mail, avec la vérification de code par mail qui set isValide à true dans la bdd, puis continuer la logique de register/login
        throw new NotImplementedException();
    }

    public async Task<IResponseDataModel<ClientOutputDto>> Login(string email, string motDePasse)
    {
        try 
        {
            var Client = await GetClientByEmail(email);
            if (Client == null)
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Message = "Email ou mot de passe incorrect",
                    Success = false
                };
            }

            if (!_hash.VerifyMotDePasse(motDePasse, Client.MotDePasse))
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Mot de passe incorrect"
                };
            }
            
            var tokenJWT = _jwtService.GenererToken(_mapper.Map<ClientInputDto>(Client));
            Client.AcessToken = tokenJWT;
            
            var outputDto = _mapper.Map<ClientOutputDto>(Client);

            return new ResponseDataModel<ClientOutputDto>
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
            return new ResponseDataModel<ClientOutputDto>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la connexion"
            };
        }
    }

    public async Task<IResponseDataModel<string>> ResetMotDePasse(string email)
    {
        try 
        {
            var Client = await GetClientByEmail(email);
            if (Client == null)
            {
                return new ResponseDataModel<string>
                {
                    Message = "Client non trouvé",
                    Success = false
                };
            }

            var derniereDemande = await _context.ReinitialisationMdps
                .Where(r => r.ClientId == Client.ClientId)
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
            
            Client.MotDePasse = motDePasseHash;
            
            var nouvelleDemande = new ReinitialisationMdp
            {
                ClientId = Client.ClientId,
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
}