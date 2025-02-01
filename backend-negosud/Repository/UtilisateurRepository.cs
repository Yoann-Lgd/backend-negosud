using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class UtilisateurRepository : RepositoryBase<Utilisateur>, IUtilisateurRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<UtilisateurRepository> _logger;

    public UtilisateurRepository(
        PostgresContext context, 
        IMapper mapper, 
        ILogger<UtilisateurRepository> logger):base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Utilisateurs
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
        public async Task<ResponseDataModel<UtilisateurOutputDto>> AddAsync(Utilisateur entity,
            CancellationToken cancellationToken = default)
        {
            try 
            {
                var utilisateur = _mapper.Map<Utilisateur>(entity);
                await _context.Utilisateurs.AddAsync(utilisateur);
                await _context.SaveChangesAsync();
                var userOutput = _mapper.Map<UtilisateurOutputDto>(utilisateur);
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = true,
                    Data = userOutput
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la création de l'utilisateur");
                throw;
            }
        }

        public Task<ResponseDataModel<UtilisateurOutputDto>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDataModel<List<UtilisateurOutputDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Utilisateur entity, CancellationToken cancellationToken = default)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(Utilisateur entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
}
