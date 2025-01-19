using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Services;

public class VerificationEmailService : IVerificationEmailService
{
    private readonly PostgresContext _context;
    
    public VerificationEmailService(PostgresContext context)
    {
        _context = context; 
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Utilisateurs
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}



