using backend_negosud.Enums;

namespace backend_negosud.DTOs;

public class CommandeOutputDto
{
    public int CommandeId { get; set; }
    public int ClientId { get; set; }
    public DateTime DateCreation { get; set; }
    public decimal Total { get; set; }
    public int Quantite { get; set; }
    public StatutCommande Statut { get; set; }
    public bool Valide { get; set; }
    public List<LigneCommandeOutputDto> LigneCommandes { get; set; } 
}