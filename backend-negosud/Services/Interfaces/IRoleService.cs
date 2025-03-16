using backend_negosud.DTOs.Role;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IRoleService
{
    Task<IResponseDataModel<List<RoleCompleteDto>>> GetRoles();
}