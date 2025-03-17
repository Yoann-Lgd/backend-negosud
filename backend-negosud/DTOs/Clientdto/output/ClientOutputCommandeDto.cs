using backend_negosud.DTOs.Commande_client.Outputs;

namespace backend_negosud.DTOs;

public class ClientOutputCommandeDto
{
    public int ClientId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string Tel { get; set; } = null!;
    
    public bool EstValide { get; set; }
    public List<CommandeOutputDto> Commandes { get; set; }
}