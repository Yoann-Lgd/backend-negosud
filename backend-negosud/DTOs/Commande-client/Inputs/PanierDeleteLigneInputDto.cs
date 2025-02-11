using backend_negosud.Entities;

namespace backend_negosud.DTOs.Commande_client;

public class PanierDeleteLigneInputDto
{
    public int CommandId { get; set; }
    public int ClientId { get; set; }
    public int LigneCommandeId { get; set; }
}