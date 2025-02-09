namespace backend_negosud.DTOs.Adresse.AdresseInputDto;

public class AdresseCreateInputDto
{
    public int Numero { get; set; }

    public string Ville { get; set; } = null!;

    public int CodePostal { get; set; }

    public string Departement { get; set; } = null!;

    public int PaysId { get; set; }
}