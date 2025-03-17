using AutoMapper;
using backend_negosud.DTOs.Pays.PaysOutputDto;
using backend_negosud.DTOs.Role;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RoleService> _logger;
    private readonly IMapper _mapper;
    
    public RoleService(IRoleRepository roleRepository,ILogger<RoleService> logger, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<IResponseDataModel<List<RoleCompleteDto>>> GetRoles()
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            
            var roleCompleteDtos = _mapper.Map<List<RoleCompleteDto>>(roles);
            
            return new ResponseDataModel<List<RoleCompleteDto>>
            {
                Success = true,
                Message = "Rôles récupérés avec succès.",
                StatusCode = 200,
                Data = roleCompleteDtos
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la récupération des rôles");

            return new ResponseDataModel<List<RoleCompleteDto>>
            {
                Success = false,
                Message = "Une erreur s'est produite lors de la récupération des rôles.",
                StatusCode = 500,
                Data = new List<RoleCompleteDto>()
            };
        }
    }
}