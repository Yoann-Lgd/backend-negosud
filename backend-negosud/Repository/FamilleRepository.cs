using AutoMapper;
using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class FamilleRepository : RepositoryBase<Famille>, IFamilleRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<FamilleRepository> _logger;
    
    public FamilleRepository(
        PostgresContext context,
        IMapper mapper, 
        ILogger<FamilleRepository> logger):base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<List<Famille>> GetAllFamillesAsync()
    {
        return await _context.Familles
            .Include(f => f.Articles)
            .ToListAsync();
    }
    
    public async Task<Famille> GetFamilleByIdAndArticles(int id)
    {
        return await _context.Familles.Include(f => f.Articles).FirstOrDefaultAsync(f => f.FamilleId == id);
    }
}