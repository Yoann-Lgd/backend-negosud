using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IClientService : IAuthService<Client, ClientInputDto, ClientOutputDto>
{

    Task<Client> GetClientByToken(string token);

    Task UpdateClient(Client client);

    Task <Client>GetClientByEmail(string email);

    Task<bool> ValidationClientEmail(ClientInputDto clientInputDto);
   Task<IResponseDataModel<string>> VerifierCodeValidation(string email, string code);
  Task<IResponseDataModel<ClientOutputDto>> CreateClient(ClientInputDto ClientInputDto);

    Task<IResponseDataModel<ClientOutputDto>> Login(string email, string motDePasse);
    Task<IResponseDataModel<Client>> GetClientBydId(int id);

    Task<IResponseDataModel<string>> ResetMotDePasse(string email);
}