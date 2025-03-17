using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class ClientRepository : RepositoryBase<Client>,IClientRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<ClientRepository> _logger;
    
    public ClientRepository(
        PostgresContext context, 
        IMapper mapper, 

        ILogger<ClientRepository> logger) : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Clients
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }

public async Task<Client?> GetClientBydIdComandes(int id)
{
    return await _context.Clients
        .Where(c => c.ClientId == id)
        .Include(client => client.Commandes)
        .FirstOrDefaultAsync();
}
}