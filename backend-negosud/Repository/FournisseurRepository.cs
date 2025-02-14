using AutoMapper;
using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class FournisseurRepository : RepositoryBase<Fournisseur>, IFournisseurRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<CommandeRepository> _logger;
    
    public FournisseurRepository(PostgresContext context, 
        IMapper mapper, 
        ILogger<CommandeRepository> logger) : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Fournisseur>> GetAllFournisseursAsync()
    {
        return await _context.Fournisseurs
            .Include(f => f.Articles)
            .Include(f => f.Adresses)
            .ToListAsync();
    }
    
}