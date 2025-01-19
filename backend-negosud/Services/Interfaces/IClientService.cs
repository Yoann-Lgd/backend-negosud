using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IClientService
{
    Task<IResponseDataModel<ClientOutputDto>> CreateClient(Client client);
    

    Task <Client>GetClientByEmail(string email);
}