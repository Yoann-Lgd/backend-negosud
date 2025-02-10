using AutoMapper;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.DTOs.Pays.PaysInputDto;
using backend_negosud.DTOs.Pays.PaysOutputDto;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Validation.Pays;
using FluentValidation.Results;

namespace backend_negosud.Services;

public class PaysService : IPaysService
{
    private IPaysRepository _paysRepository;
    private readonly ILogger<UtilisateurService> _logger;
    private readonly IMapper _mapper;

    public PaysService(IPaysRepository paysRepository,ILogger<UtilisateurService> logger, IMapper mapper)
    {
        _paysRepository = paysRepository;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<IResponseDataModel<string>> CreatePays(PaysInputDto paysInputDto)
    {
        try
        {
                var validator = new PaysCreateValidation();
                ValidationResult validationResult = validator.Validate(paysInputDto);
                
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation des données d'entrée échouée : {Errors}", string.Join(", ", errors));
                    return new ResponseDataModel<string>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = string.Join(", ", errors),
                    };
                }

                var pays = _mapper.Map<Pays>(paysInputDto);
                var paysCreated = await _paysRepository.AddAsync(pays);
                
                return new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Pays créée avec succès.",
                    StatusCode = 201,
                    Data = paysCreated.PaysId.ToString(),
                };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création du pays");

            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la création du pays.",
                StatusCode = 500,
            };
        }
    }
    
    public async Task<IResponseDataModel<PaysEssentialOutputDto>> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour récupérer le pays : {Id}", id);
                return new ResponseDataModel<PaysEssentialOutputDto>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }

            var pays = await _paysRepository.GetByIdAsync(id);
            if (pays == null)
            {
                _logger.LogWarning("Pays non trouvé pour l'identifiant : {Id}", id);
                return new ResponseDataModel<PaysEssentialOutputDto>
                {
                    Success = false,
                    Message = "Pays non trouvé.",
                    StatusCode = 404,
                };
            }

            var output = _mapper.Map<PaysEssentialOutputDto>(pays);
            return new ResponseDataModel<PaysEssentialOutputDto>
            {
                Success = true,
                Message = "Pays récupéré avec succès",
                Data = output,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération du pays");

            return new ResponseDataModel<PaysEssentialOutputDto>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération du pays.",
                StatusCode = 500,
            };
        }
    }
    
    public async Task<IResponseDataModel<List<PaysEssentialOutputDto>>> GetAllPays()
    {
        try
        {
            var pays = await _paysRepository.GetAllAsync();
            
            var paysOutputDtos = _mapper.Map<List<PaysEssentialOutputDto>>(pays);
            
            return new ResponseDataModel<List<PaysEssentialOutputDto>>
            {
                Success = true,
                Message = "Pays récupérées avec succès.",
                StatusCode = 200,
                Data = paysOutputDtos
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des pays");

            return new ResponseDataModel<List<PaysEssentialOutputDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des pays.",
                StatusCode = 500,
                Data = new List<PaysEssentialOutputDto>()
            };
        }
    }

    public async Task<IResponseDataModel<string>> DeletePays(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Identifiant invalide fourni pour supprimer le pays : {Id}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Identifiant invalide fourni.",
                    StatusCode = 400,
                };
            }
            
            var pays = await _paysRepository.GetByIdAsync(id);
            if (pays == null)
            {
                _logger.LogWarning("Pays non trouvé pour l'identifiant : {Id}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Pays non trouvé.",
                    StatusCode = 404,
                };
            }

            await _paysRepository.DeleteAsync(pays);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "Pays supprimé.",
                StatusCode = 200,
                Data = id.ToString(),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression du pays");

            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression du pays.",
                StatusCode = 500,
            };
        }
    }
}