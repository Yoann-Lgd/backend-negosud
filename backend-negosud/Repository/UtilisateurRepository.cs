using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Mapper;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly PostgresContext _context;
    private readonly IMapper _mapper;
    public UtilisateurRepository(PostgresContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto utilisateurInputDto)
    {

        try
        {
            var mail = utilisateurInputDto.Email;
            
            if (string.IsNullOrEmpty(utilisateurInputDto.Email) || string.IsNullOrEmpty(utilisateurInputDto.MotDePasse))
            {
                Console.WriteLine("UtilisateurInputDto : " + utilisateurInputDto);
                return new  ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email ou mot de passe non spécifié"
                };
            }
            
            

            
            var mailExistant = await _context.Utilisateurs.AnyAsync(u => u.Email == mail);
            if (mailExistant)
            {
                return new ResponseDataModel<UtilisateurOutputDto>
                {
                    Success = false,
                    Message = "Email déjà utilisé"
                };
            }

            // Mapper directement de InputDto vers l'entité Utilisateur
            var utilisateur = _mapper.Map<Utilisateur>(utilisateurInputDto);
            Console.WriteLine(utilisateur);
            await _context.Utilisateurs.AddAsync(utilisateur);
            await _context.SaveChangesAsync();

// Mapper l'entité vers OutputDto pour la réponse
            var userOutput = _mapper.Map<UtilisateurOutputDto>(utilisateur);

            return new ResponseDataModel<UtilisateurOutputDto>
            {
                Success = true,
                Data = userOutput
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