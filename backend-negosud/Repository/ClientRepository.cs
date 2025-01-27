using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class ClientRepository : IClientRepository
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;

    private readonly ILogger<ClientRepository> _logger;
    
    public ClientRepository(
        PostgresContext context, 
        IMapper mapper, 

        ILogger<ClientRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<ResponseDataModel<ClientOutputDto>> AddAsync(Client entity, CancellationToken cancellationToken = default)
    {
        try 
        {
            var client = _mapper.Map<Client>(entity);
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            var clientOutput = _mapper.Map<ClientOutputDto>(client);
            return new ResponseDataModel<ClientOutputDto>
            {
                Success = true,
                Data = clientOutput
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur lors de la création du client");
            throw;
        }
    }

    public Task<ResponseDataModel<ClientOutputDto>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDataModel<List<ClientOutputDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Client entity, CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Clients
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }
}