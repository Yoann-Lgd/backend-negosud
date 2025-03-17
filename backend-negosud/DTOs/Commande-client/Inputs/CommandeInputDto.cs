using backend_negosud.DTOs.Livraison.Inputs;

namespace backend_negosud.DTOs.Commande_client;

public class CommandeInputDto
{

    public int ClientId { get; set; }
    
    public List<LigneCommandeCreateInputDto> LigneCommandes { get; set; }
    public LivraisonInputCommandeDto Livraison { get; set; } 
}