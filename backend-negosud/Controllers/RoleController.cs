using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> getRoles()
    {
        var result = await _roleService.GetRoles();
        return result.Success ? Ok(result) : BadRequest(result);
    }        
}