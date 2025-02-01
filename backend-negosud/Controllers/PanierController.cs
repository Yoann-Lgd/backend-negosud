using backend_negosud.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PanierController : ControllerBase
{
    private readonly ICommandeService _commandeService;
    
    public PanierController(ICommandeService commandeService)
    {
        _commandeService = commandeService;
    }
    
    
}