using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class UtilisateurService : IUtilisateurService
{
    private readonly IUtilisateurRepository _utilisateurRepository;
    

    public UtilisateurService(IUtilisateurRepository utilisateurRepository)
    {
        _utilisateurRepository = utilisateurRepository;
       
    }

    public async Task<IResponseModel> CreateUtilisateur(UtilisateurInputDto utilisateur)
    {
        try
        {
          
            return await _utilisateurRepository.CreateAsync(utilisateur);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}