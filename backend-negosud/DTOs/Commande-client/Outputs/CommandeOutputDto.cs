using backend_negosud.DTOs.Livraison.Outputs;

namespace backend_negosud.DTOs.Commande_client.Outputs;

public class CommandeOutputDto
{
    public int CommandeId { get; set; }
    public DateTime DateCreation { get; set; }
    public bool Valide { get; set; }
    public int ClientId { get; set; }
    public int? LivraisonId { get; set; }
    public int? FactureId { get; set; }
    public LivraisonOutputDto livraison { get; set; }
    public List<LigneCommandeOutputDto> LigneCommandes { get; set; }
}