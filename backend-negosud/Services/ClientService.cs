using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation;

namespace backend_negosud.Services;

public class ClientService : IClientService
{
    private readonly IVerificationEmailService _verificationEmail;
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IHashMotDePasseService _hash;
    private readonly ILogger<ClientService> _logger;
    private readonly IClientRepository _clientRepository;
    
    public ClientService(IClientRepository clientRepository, IVerificationEmailService verificationEmail, PostgresContext context, IMapper mapper, IJwtService jwtService, IHashMotDePasseService hash, ILogger<ClientService> logger)
    {
        
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
        _hash = hash;
        _logger = logger;
        _verificationEmail = verificationEmail;
        _clientRepository = clientRepository;
    }
    public async Task<IResponseDataModel<ClientOutputDto>> CreateClient(Client client)
    {
                try
        {
            // Validation des entrées
            if (string.IsNullOrEmpty(client.Email) || string.IsNullOrEmpty(client.MotDePasse))
            {
                _logger.LogWarning("Tentative de création d'utilisateur avec email ou mot de passe vide");
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Email ou mot de passe non spécifié"
                };
            }

            // Vérification email existant
            if (await _verificationEmail.EmailExistsAsync(client.Email))
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }

            // Validation
            var validation = new ClientValidation();
            var result = validation.Validate(client);

            if (!result.IsValid)
            {
                return new ResponseDataModel<ClientOutputDto>
                {
                    Success = false,
                    Message = "Données utilisateur invalides: " + string.Join(", ", result.Errors)
                };
            }

            // Création utilisateur
           
            client.MotDePasse = _hash.HashMotDePasse(client.MotDePasse);

            // Sauvegarde via repository
            await _repository.AddAsync(client);

            // Génération token
            var tokenJWT = _jwtService.GenererToken(client as T);
            client.AccessToken = tokenJWT;

            await _repository.UpdateAsync(client);

            var clientOutput = _mapper.Map<ClientOutputDto>(client);

            return new ResponseDataModel<ClientOutputDto>
            {
                TokenJWT = tokenJWT,
                Success = true,
                Data = clientOutput
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création de l'utilisateur");
            throw;
        }
    }

    public Task<Client> GetClientByEmail(string email)
    {
        throw new NotImplementedException();
    }
}