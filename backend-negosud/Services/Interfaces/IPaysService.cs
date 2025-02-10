using backend_negosud.DTOs.Pays.PaysInputDto;
using backend_negosud.DTOs.Pays.PaysOutputDto;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IPaysService
{
    Task<IResponseDataModel<string>> CreatePays(PaysInputDto paysInputDto);
    Task<IResponseDataModel<PaysEssentialOutputDto>> GetById(int id);
    Task<IResponseDataModel<List<PaysEssentialOutputDto>>> GetAllPays();
    Task<IResponseDataModel<string>> DeletePays(int id);
}