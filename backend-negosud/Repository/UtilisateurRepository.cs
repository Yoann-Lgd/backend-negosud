using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;
    public UtilisateurRepository(PostgresContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto utilisateurInputDto)
    {
        try
        {
            
            if (string.IsNullOrEmpty(utilisateurInputDto.Email) || string.IsNullOrEmpty(utilisateurInputDto.MotDePasse))
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email ou mot de passe non spécifié"
                };
            }

            
            var mailExistant = await _context.Utilisateurs.AnyAsync(u => u.Email == utilisateurInputDto.Email);
            if (mailExistant)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }

            
            var entityEntry = new Utilisateur
            {
                Email = utilisateurInputDto.Email,
                MotDePasse = utilisateurInputDto.MotDePasse,
                AccessToken = utilisateurInputDto.access_token,
                Role = _mapper.Map<Role>(utilisateurInputDto.RoleDto)
                
            };

            await _context.Utilisateurs.AddAsync(entityEntry);
            await _context.SaveChangesAsync();
            var newUser = _mapper.Map<UtilisateurOutputDto>(entityEntry);
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = true,
                Data = newUser
            };
            
            
        }
        catch (Exception e)
        {
            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = false,
                Message = $"Une erreur est survenue : {e.Message}"
            };
        }
    }
}