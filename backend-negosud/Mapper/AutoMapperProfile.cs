using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;

namespace backend_negosud.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Utilisateur, UtilisateurOutputDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));
        
        CreateMap<UtilisateurInputDto, Utilisateur>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.access_token))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Nom))
            .ForMember(dest => dest.Prenom, opt => opt.MapFrom(src => src.Prenom));
        
        CreateMap<Utilisateur, UtilisateurInputDto>();

   
        CreateMap<Adresse, AdresseDto>();
        
        CreateMap<RoleDto, Role>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));
        
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));

        CreateMap<Stock, StockHistoryDto>();

        CreateMap<Stock, StockDetailDto>();

        CreateMap<Stock, StockSummaryDto>();

        CreateMap<Stock, StockInputDto>();
    }
}
