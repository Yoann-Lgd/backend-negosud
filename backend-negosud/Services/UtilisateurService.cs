using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Models;
using backend_negosud.Repository;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public class UtilisateurService : IUtilisateurService
{
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly PostgresContext _context;

    public UtilisateurService(IUtilisateurRepository utilisateurRepository, PostgresContext context)
    {
        _utilisateurRepository = utilisateurRepository;
        _context = context;
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

    public async Task<Utilisateur> GetUtilisateurByToken(string token)
    {
        return await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.AccessToken.ToString() == token);
    }

    public async Task UpdateUtilisateur(Utilisateur utilisateur)
    {
        _context.Utilisateurs.Update(utilisateur);
        await _context.SaveChangesAsync();
    }

    public async Task<Utilisateur> GetUtilisateuByEmail(string email)
    {
        return await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);
    }
}