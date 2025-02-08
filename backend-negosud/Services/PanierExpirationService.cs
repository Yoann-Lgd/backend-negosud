using backend_negosud.Entities;

namespace backend_negosud.Services;

public class PanierExpirationService
{
    private readonly PostgresContext _context;

    public PanierExpirationService(PostgresContext context)
    {
        _context = context;
    }

    public void CleanupExpiredBaskets()
    {
        var currentDate = DateTime.UtcNow;
        var expiredBaskets = _context.Commandes
            .Where(c => !c.Valide && c.ExpirationDate < currentDate)
            .ToList();

        _context.Commandes.RemoveRange(expiredBaskets);
        _context.SaveChanges();
    }
}