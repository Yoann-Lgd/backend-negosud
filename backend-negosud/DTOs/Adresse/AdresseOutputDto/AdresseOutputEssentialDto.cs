using backend_negosud.DTOs.Pays.PaysOutputDto;

namespace backend_negosud.DTOs.Adresse.AdresseOutputDto;

public class AdresseOutputEssentialDto
{
    public int AdresseId { get; set; }

    public int Numero { get; set; }

    public string Ville { get; set; }

    public int CodePostal { get; set; }

    public string Departement { get; set; }

    public PaysEssentialOutputDto PaysEssentialOutputDto { get; set; }
}