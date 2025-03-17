using backend_negosud.Entities;

namespace backend_negosud.Repository.Interfaces;

public interface IFournisseurRepository : IRepositoryBase<Fournisseur>
{
    Task<List<Fournisseur>> GetAllFournisseursAsync();
    
    Task<List<Fournisseur>> GetAllFournisseursNotDeleteAsync();
}