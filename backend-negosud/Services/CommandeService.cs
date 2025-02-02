using AutoMapper;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class CommandeService : ICommandeService
{
    private ICommandeRepository _commandeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommandeService> _logger;

    public CommandeService(IMapper mapper, ICommandeRepository commandeRepository, ILogger<CommandeService> logger)
    {
        _commandeRepository = commandeRepository;
        _mapper = mapper;
        _logger = logger;
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
                StatusCode = 500,
                Data = new List<CommandeOutputDto>()
            };
        }
    }
}