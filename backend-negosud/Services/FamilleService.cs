using AutoMapper;
using backend_negosud.DTOs.Famille;
using backend_negosud.DTOs.Famille.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Validation;

namespace backend_negosud.Services;

public class FamilleService : IFamilleService
{
    private readonly IMapper _mapper;
    private readonly ILogger<FamilleService> _logger;
    private readonly IFamilleRepository _familleRepository;

    public FamilleService(IMapper mapper, ILogger<FamilleService> logger, IFamilleRepository familleRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _familleRepository = familleRepository;
    }
    
     public async Task<IResponseDataModel<List<FamilleOutputDto>>> GetAllFamilles()
        {
            try
            {
                var familles = await _familleRepository.GetAllFamillesAsync();
                var famillesOutputDtos = _mapper.Map<List<FamilleOutputDto>>(familles);
                return new ResponseDataModel<List<FamilleOutputDto>>
                {
                    Data = famillesOutputDtos,
                    Success = true,
                    Message = "Familles récupérées avec succès."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des familles.");
                return new ResponseDataModel<List<FamilleOutputDto>>
                {
                    Success = false,
                    Message = "Une erreur s'est produite lors de la récupération des familles."
                };
            }
        }

        public async Task<IResponseDataModel<FamilleOutputDto>> getFamilleById(int id)
        {
            try
            {
                var famille = await _familleRepository.GetFamilleByIdAndArticles(id);
                var familleOutputDto = _mapper.Map<FamilleOutputDto>(famille);
                return new ResponseDataModel<FamilleOutputDto>          
                {
                    Data = familleOutputDto,
                    Success = true,
                    Message = "Familles récupérées avec succès."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la famille avec l'ID {id}.");                
                return new ResponseDataModel<FamilleOutputDto>
                {
                    Success = false,
                    Message = "Une erreur s'est produite lors de la récupération de la famille."
                };
            }
        }
        

        public async Task<IResponseDataModel<string>> CreateFamille(FamilleCreateInputDto familleDto)
        {
            try
            {
                var validation = new FamilleValidation();
                var result = validation.Validate(familleDto);
                if (!result.IsValid)
                {
                    return new ResponseDataModel<string>
                    {
                        Success = false,
                        Message = "Données utilisateur invalides: " + string.Join(", ", result.Errors)
                    };
                }
                
                var famille = _mapper.Map<Famille>(familleDto);
                var familleId = await _familleRepository.AddAsync(famille);
                return new ResponseDataModel<string>
                {
                    Data = familleId.FamilleId.ToString(),
                    Success = true,
                    Message = "Famille créée avec succès."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la famille.");
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Une erreur s'est produite lors de la création de la famille."
                };
            }
        }
}
    
