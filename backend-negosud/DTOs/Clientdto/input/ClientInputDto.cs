namespace backend_negosud.DTOs;

public class ClientInputDto
{
    public int ClientId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public string Tel { get; set; } = null!;
    
    public string AcessToken { get; set; } = null!;
    
}

public class ClientInputDtoSimplified
{
    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;
}