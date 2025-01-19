using backend_negosud.Controllers;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Repository;

public interface IClientRepository : IRepositoryBase<Client, ClientOutputDto>
{

}