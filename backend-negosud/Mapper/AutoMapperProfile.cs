using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.entities;

namespace backend_negosud.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Utilisateur, UtilisateurOutputDto>();
        CreateMap<UtilisateurInputDto, Utilisateur>();
        CreateMap<Adresse, AdresseDto>();
    }
}