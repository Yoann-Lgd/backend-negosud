using AutoMapper;
using backend_negosud.Entities;

namespace backend_negosud.Repository;

public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<ClientRepository> _logger;
    
    public ArticleRepository(
        PostgresContext context, 
        IMapper mapper, 

        ILogger<ClientRepository> logger) : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}