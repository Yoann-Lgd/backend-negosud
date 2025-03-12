using AutoMapper;
using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using backend_negosud.Validation.Commande;
using FluentValidation.Results;

namespace backend_negosud.Services;

public class CommandeService : ICommandeService
{
    private ICommandeRepository _commandeRepository;
    private IArticleRepository _articleRepository;
    private IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommandeService> _logger;

    public CommandeService(IMapper mapper, ICommandeRepository commandeRepository, IArticleRepository articleRepository, IClientRepository clientRepository, ILogger<CommandeService> logger)
    {
        _commandeRepository = commandeRepository;
        _clientRepository = clientRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<IResponseDataModel<CommandeOutputDto>> CreateCommande(CommandeInputDto commandeInput)
    {
        try
        {
            var validator = new CommandeCreateValidation();
            ValidationResult validationResult = validator.Validate(commandeInput);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors),
                };
            }

            _logger.LogInformation("Tentative de récupération du client avec ID: {ClientId}", commandeInput.ClientId);
            var client = await _clientRepository.GetByIdAsync(commandeInput.ClientId);

            if (client == null)
            {
                _logger.LogError("Client introuvable pour l'ID: {ClientId}", commandeInput.ClientId);
                return new ResponseDataModel<CommandeOutputDto>
                {
                    Success = false,
                    Message = "Client introuvable.",
                    StatusCode = 404
                };
            }

            var ligneCommandes = _mapper.Map<List<LigneCommande>>(commandeInput.LigneCommandes);
            foreach (var ligne in ligneCommandes)
            {
                var articleExist = await _articleRepository.AnyAsync(a => a.ArticleId == ligne.ArticleId);
                if (!articleExist)
                {
                    _logger.LogWarning("Article ID {ArticleId} n'existe pas.", ligne.ArticleId);
                }
            }

            var commande = _mapper.Map<Commande>(commandeInput);
            commande.DateCreation = DateTime.UtcNow;
            commande.LigneCommandes = ligneCommandes;

            var livraisonInputDto = commandeInput.Livraison;
            var livraison = _mapper.Map<Livraison>(livraisonInputDto);
            livraison.DateEstimee = DateTime.UtcNow.AddDays(5);
            livraison.Livree = false;

            await _commandeRepository.AddAsync(commande);

            var createdCommande = await _commandeRepository.GetByIdAndLigneCommandesAsync(commande.CommandeId);

            var outputDto = _mapper.Map<CommandeOutputDto>(createdCommande);

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = true,
                Message = "Commande créée avec succès.",
                StatusCode = 201,
                Data = outputDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création de la commande");

            return new ResponseDataModel<CommandeOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la création de la commande.",
                StatusCode = 500,
                Data = null,
            };
        }
    }


    public async Task<CommandeOutputDto> GetCommandeById(int id)
    {
        var commande = await _commandeRepository.GetByIdAndLigneCommandesAsync(id);
        var commandeOutputDto = _mapper.Map<CommandeOutputDto>(commande);
        return commandeOutputDto;
    }

    public async Task<IResponseDataModel<List<CommandeOutputDto>>> GetAllCommandes()
    {
        try
        {
            var commandes = await _commandeRepository.GetAllCommandeAsync();
            
            var commandeOutputDtos = _mapper.Map<List<CommandeOutputDto>>(commandes);
            
            return new ResponseDataModel<List<CommandeOutputDto>>
            {
                Success = true,
                Message = "Commandes récupérées avec succès.",
                StatusCode = 200,
                Data = commandeOutputDtos
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des commandes");

            return new ResponseDataModel<List<CommandeOutputDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des commandes.",
                StatusCode = 500
            };
        }
    }

    public async Task<IResponseDataModel<string>> SoftDeleteAsync(int commandeId)
    {
        try
        {
            if (commandeId <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer la commande : {Id}", commandeId);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }

            var commande = await _commandeRepository.GetByIdAsync(commandeId);
            if (commande == null)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }

            if (!commande.Valide)
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande est à l'état de panier.",
                    StatusCode = 400,
                    Data = commandeId.ToString(),
                };
            }

            var response = await _commandeRepository.SoftDeleteEntityByIdAsync<Commande, int>(commandeId);
            if (response)
            {
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "La commande a été soft-supprimée.",
                    StatusCode = 200,
                    Data = commandeId.ToString(),
                };
            }
            else
            {
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "La commande n'a pas été trouvée.",
                    StatusCode = 404,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du soft delete de la commande");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur est survenue lors du soft delete de la commande.",
                StatusCode = 500,
            };
        }
    }

}