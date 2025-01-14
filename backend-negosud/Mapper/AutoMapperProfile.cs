using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.entities;

namespace backend_negosud.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Utilisateur, UtilisateurOutputDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.RoleId));

        CreateMap<UtilisateurInputDto, Utilisateur>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.access_token))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleDto.RoleId));

        CreateMap<Adresse, AdresseDto>();
        CreateMap<RoleDto, Role>();
        CreateMap<Role, RoleDto>();
    }
}