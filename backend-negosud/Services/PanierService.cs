using backend_negosud.DTOs.Commande_client;
using backend_negosud.Entities;
using backend_negosud.Models;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public class PanierService(ICommandeRepository commandeRepository)
{
    // public async 
    // Task<IResponseDataModel<Commande>> CreatePanier(PanierCreateInputDto panierCreateInputDto)
    // {
    //     
    //     // return new IResponseDataModel<Commande>
    //     // {
    //     //     Success = true,
    //     //     Data = panierCreateOutput,
    //     // };
    // }
}