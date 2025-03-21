using AutoMapper;
using backend_negosud.DTOs.Fournisseur.FournisseurInputDto;
using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository.Interfaces;
using backend_negosud.Validation.Fournisseur;
using FluentValidation.Results;

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
    
   public async Task<IResponseDataModel<string>> PatchMinimalFournisseur(int id, FournisseurInputMinimal fournisseurInputMinimal)
{
    if (id != null)
    {
        fournisseurInputMinimal.FournisseurId = id;
    }
    
    var validator = new FournisseurValidation();
    ValidationResult validationResult = validator.Validate(fournisseurInputMinimal);

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

    try
    {
        var fournisseur = await _fournisseurRepository.GetByIdAsync(id);
        if (fournisseur == null)
        {
            return new ResponseDataModel<string>
            {
                Success = false,
                StatusCode = 404,
                Message = "Fournisseur non trouvé.",
            };
        }

        // mis à jour uniquement des champs fournis
        if (!string.IsNullOrEmpty(fournisseurInputMinimal.Email))
        {
            fournisseur.Email = fournisseurInputMinimal.Email;
        }

        if (!string.IsNullOrEmpty(fournisseurInputMinimal.Nom))
        {
            fournisseur.Nom = fournisseurInputMinimal.Nom;
        }

        if (!string.IsNullOrEmpty(fournisseurInputMinimal.Tel))
        {
            fournisseur.Tel = fournisseurInputMinimal.Tel;
        }

        if (!string.IsNullOrEmpty(fournisseurInputMinimal.RaisonSociale))
        {
            fournisseur.RaisonSociale = fournisseurInputMinimal.RaisonSociale;
        }

        await _fournisseurRepository.UpdateAsync(fournisseur);

        return new ResponseDataModel<string>
        {
            Success = true,
            StatusCode = 200,
            Message = "Le fournisseur a été mis à jour avec succès.",
        };
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Une erreur s'est produite lors de la mise à jour du fournisseur.");
        return new ResponseDataModel<string>
        {
            Success = false,
            StatusCode = 500,
            Message = "Une erreur s'est produite lors de la mise à jour du fournisseur.",
        };
    }
}

public async Task<IResponseDataModel<string>> CreateFournisseur(FournisseurInputMinimal fournisseurInputMinimal)
{
    try
    {
        var nouveauFournisseur = _mapper.Map<Fournisseur>(fournisseurInputMinimal);
        var createdFournisseur = await _fournisseurRepository.AddAsync(nouveauFournisseur);
    
        if (createdFournisseur == null)
        {
            _logger.LogError("Fournisseur vide");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Renseignez un fournisseur non vide",
                StatusCode = 404
            };
        }
    
        return new ResponseDataModel<string>
        {
            Success = true,
            Message = "Fournisseur créé avec succès",
            StatusCode = 200
        };
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Erreur lors de la création du fournisseur.");
        return new ResponseDataModel<string>
        {
            Success = false,
            Message = "Une erreur s'est produite lors de la création du fournisseur.",
            StatusCode = 500,
            
        };
    }
    
}

public async Task<IResponseDataModel<FournisseurOutputCompleteDto>> GetFournisseurById(int id)
{
    try
    {
        var fournisseur = await _fournisseurRepository.GetByIdAsync(id);
        if (fournisseur == null)
        {
            _logger.LogError("fournisseur introuvable pour l'ID: {fournisseurId}", id);
            return new ResponseDataModel<FournisseurOutputCompleteDto>
            {
                Success = false,
                Message = "fournisseur introuvable.",
                StatusCode = 404
            };
        }
        

        return new ResponseDataModel<FournisseurOutputCompleteDto>
        {
            Success = true,
            Message = "fournisseur récupéré",
            StatusCode = 200,
            Data = _mapper.Map<FournisseurOutputCompleteDto>(fournisseur)
        };
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Erreur lors de la récupération du fournisseur.");
        return new ResponseDataModel<FournisseurOutputCompleteDto>
        {
            Success = false,
            Message = "Une erreur s'est produite lors de la récupération du fournisseur.",
            StatusCode = 500
        };
    }
}


public async Task<IResponseDataModel<string>> softDeleteFournisseurById(int id)
    {
        try
        {
            var fournisseur = await _fournisseurRepository.GetByIdAsync(id);
            if (fournisseur == null)
            {
                _logger.LogError("fournisseur introuvable pour l'ID: {fournisseurId}", id);
                return new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "fournisseur introuvable.",
                    StatusCode = 404
                };
            }

            await _fournisseurRepository.SoftDeleteEntityByIdAsync<Fournisseur, int>(id);

            return new ResponseDataModel<string>
            {
                Success = true,
                Message = "fournisseur supprimé avec succès (sofdelete).",
                StatusCode = 200,
                Data = id.ToString()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la suppression du fournisseur.");
            return new ResponseDataModel<string>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la suppression du fournisseur.",
                StatusCode = 500,
                Data = id.ToString()
            };
        }
    }
    
    
    
    
}