using AutoMapper;
using backend_negosud.Entities;

namespace backend_negosud.Repository;

public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    private readonly IMapper _mapper;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(PostgresContext context, ILogger<RoleRepository> logger, IMapper mapper) : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
