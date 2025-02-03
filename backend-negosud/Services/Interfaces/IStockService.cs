using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_negosud.Services;

public interface IStockService
{
    Task<IResponseDataModel<Stock>> AddArticleToStock(int articleId, int quantite, string refLot, int seuilMinimum,
        bool reapprovisionnementAuto);

    Task<IResponseDataModel<Stock>> UpdateStockQuantity(int stockId, int nouvelleQuantite, int utilisateurId,
        string typeModification);

    Task<IResponseDataModel<Stock>>  CheckStockLevel(int articleId, int quantiteDemandee);

    Task<IResponseDataModel<Stock>>  CheckAndReapprovisionner();
    Task<IResponseDataModel<Stock>> GetById(int id);
    Task<IResponseDataModel<String>> Delete(Stock stock);

    Task<IResponseDataModel<List<StockSummaryDto>>> GetAllStocks();

    Task<IResponseDataModel<List<Inventorier>>> GetStockHistory(int stockId);
    
}