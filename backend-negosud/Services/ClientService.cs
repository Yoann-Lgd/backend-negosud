using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;
using FluentValidation.Results;
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

    public async Task<Client> GetClientByEmail(string email)
    {
        _logger.LogInformation($"Recherche du client avec l'email : {email}");
    
        var client = await _repository.FirstOrDefaultAsync(c => c.Email == email);
    
        if (client == null)
            _logger.LogWarning($"Aucun client trouvé pour l'email : {email}");
        else
            _logger.LogInformation($"Client trouvé : {client.Email}");
    
        return client;
    }

    public async Task<IResponseDataModel<ClientOutputCommandeDto>> GetClientCommandeBydId(int id)
    {
        var client = await _repository.GetClientBydIdComandes(id);

        if (client == null)
        {
            _logger.LogError("Client introuvable pour l'ID : {ClientID}", id);
            return new ResponseDataModel<ClientOutputCommandeDto>()
            {
                Success = false,
                Message = "Commande introuvable.",
                StatusCode = 404
            };
        }
        
        var clientOutput = _mapper.Map<ClientOutputCommandeDto>(client);
        
        return new ResponseDataModel<ClientOutputCommandeDto>
        {
            StatusCode = 200,
            Data = clientOutput,
            Success = true,
        };
    }

    public async Task<bool> ValidationClientEmail(ClientInputDto clientInputDto)
    {
        try 
        {
            var client = await GetClientByEmail(clientInputDto.Email);
            if (client != null)
            {
                return false; // Email déjà utilisé
            }

            var codeTemporaire = _hash.RandomMotDePasseTemporaire();
        
            // Stocker le code temporaire
            CodeEmailDataTable.Instance.AddCodeEmail(clientInputDto.Email, codeTemporaire);

            // Préparer et envoyer l'email
            var emailSubject = "Validation de votre compte NegoSud";
            var emailBody = $@"
            <h2>Validation de votre compte NegoSud</h2>
            <p>Merci de vous être inscrit sur NegoSud. Pour valider votre compte, veuillez utiliser le code suivant :</p>
            <p><strong>{codeTemporaire}</strong></p>
            <p>Ce code est valable pendant 15 minutes.</p>
            <p>Cordialement,<br>L'équipe de NegoSud</p>";

            await _emailService.SendEmailAsync(clientInputDto.Email, emailSubject, emailBody, isHtml: true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation de l'email");
            return false;
        }
    }
    
    public async Task<IResponseDataModel<string>> VerifierCodeValidation(string email, string code)
    {
        try
        {
            var client = await GetClientByEmail(email);
            if (client == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Client non trouvé"
                };
            }

            var codeStocke = await CodeEmailDataTable.Instance.GetCodeEmail(email);

            if (codeStocke == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Code de validation expiré ou non trouvé"
                };
            }

            if (codeStocke != code)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Code de validation incorrect"
                };
            }

            // Le code est valide, on met à jour le statut du client
            client.EstValide = true;
            await UpdateClient(client);

            // Supprimer le code temporaire
            await CodeEmailDataTable.Instance.DeleteCodeEmail(email);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Email validé avec succès"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification du code de validation");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors de la validation de l'email"
            };
        }
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

        // Validation de l'email
        var emailValidationResult = await ValidationClientEmail(ClientInputDto);
        if (!emailValidationResult)
        {
            return new ResponseDataModel<ClientOutputDto>
            {
                Success = false,
                Message = "Échec de l'envoi de l'email de validation"
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
        Client.EstValide = false; // Le compte n'est pas encore validé

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
            Data = userOutput,
            Message = "Un email de validation a été envoyé à votre adresse email"
        };
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Erreur lors de la création de l'Client");
        throw;
    }
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
    
        public async Task<BooleanResponseDataModel> ClientExistEmail(ClientEmailInputDto clientEmailInputDto)
    {
        try
        {
            var validator = new ClientEmailValidation();
            
            ValidationResult validationResult = validator.Validate(clientEmailInputDto);
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

            var emailLower = clientEmailInputDto.Email.ToLower();
            var exists = await _repository.EmailExistsAsync(emailLower);

            if (exists)
            {
                return new BooleanResponseDataModel
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Un client avec cet email existe.",
                    Data = true,
                };
            }
            else
            {
                return new BooleanResponseDataModel
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Aucun client trouvé avec cet email.",
                    Data = false,
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Une erreur s'est produite lors de la vérification de lu client par email.");
            return new BooleanResponseDataModel
            {
                Success = false,
                StatusCode = 500,
                Message = "Une erreur s'est produite lors de la vérification du client.",
                Data = false,
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
    
    public async Task<IResponseDataModel<string>> SoftDeleteClientAsync(int clientId)
    {
        try
        {
            if (clientId <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer le client : {Id}", clientId);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }
            
            var utilisateur = await _repository.GetByIdAsync(clientId);
            if (utilisateur == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le client n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }

            var response = await _repository.SoftDeleteEntityByIdAsync<Client, int>(clientId);
            if (response)
            {
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Le client a été soft-supprimé.",
                    StatusCode = 200,
                    Data = clientId.ToString(),
                };
            }
            else
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Le client n'a pas été trouvé.",
                    StatusCode = 404,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du soft delete du client");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors du soft delete du client.",
                StatusCode = 500,
            };
        }
    }
}