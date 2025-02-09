namespace backend_negosud.DTOs;

public class ClientOutputDto
{
    public int ClientId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string Tel { get; set; } = null!;
    
    public string AcessToken { get; set; } = null!;

}