using AutoMapper;
using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.Models;
using backend_negosud.Repository.Interfaces;

namespace backend_negosud.Services;

public class FournisseurService : IFournisseurService
{
    private readonly IFournisseurRepository _fournisseurRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FournisseurService> _logger;

    public FournisseurService(
        IFournisseurRepository fournisseurRepository,
        IMapper mapper,
        ILogger<FournisseurService> logger)
    {
        _mapper = mapper;
        _fournisseurRepository = fournisseurRepository;
        _logger = logger;
    }


    public async Task<IResponseDataModel<List<FournisseurOutputCompleteDto>>> getAll()
    {
        var fournisseurs = await _fournisseurRepository.GetAllFournisseursAsync();
        var fournisseursOutputDtos = _mapper.Map<List<FournisseurOutputCompleteDto>>(fournisseurs);
        return new ResponseDataModel<List<FournisseurOutputCompleteDto>>
        {
            Success = true,
            Message = "Fournisseurs récupérés avec succès.",
            StatusCode = 200,
            Data = fournisseursOutputDtos,
        };
    }
}