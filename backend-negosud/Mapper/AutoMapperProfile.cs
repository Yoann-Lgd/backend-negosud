using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.DTOs.Famille;
using backend_negosud.DTOs.Famille.Outputs;
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

        CreateMap<ClientInputDto, Client>();
        CreateMap<Client, ClientInputDto>();
        CreateMap<Client, ClientOutputDto>();
        CreateMap<ClientOutputDto, Client>();
        
        CreateMap<PanierCreateInputDto, Commande>()
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Valide, opt => opt.MapFrom(_ => false)) 
            .ForMember(dest => dest.LigneCommandes, opt => opt.MapFrom(src => src.LigneCommandes));

        CreateMap<Commande, PanierCreateOutputDto>()
            .ForMember(dest => dest.LigneCommandes, opt => opt.MapFrom(src => src.LigneCommandes));

        CreateMap<LigneCommande, LigneCommandeOutputDto>();

        CreateMap<LigneCommandeCreateInputDto, LigneCommande>()
            .ForMember(dest => dest.LigneCommandeId, opt => opt.Ignore());

        
        CreateMap<Commande, CommandeOutputDto>()
            .ForMember(dest => dest.CommandeId, opt => opt.MapFrom(src => src.CommandeId))
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => src.DateCreation))
            .ForMember(dest => dest.Valide, opt => opt.MapFrom(src => src.Valide))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.LivraisonId, opt => opt.MapFrom(src => src.LivraisonId))
            .ForMember(dest => dest.FactureId, opt => opt.MapFrom(src => src.FactureId))
            .ForMember(dest => dest.LigneCommandes, opt => opt.MapFrom(src => src.LigneCommandes));

        CreateMap<LigneCommande, LigneCommandeOutputDto>()
            .ForMember(dest => dest.LigneCommandeId, opt => opt.MapFrom(src => src.LigneCommandeId))
            .ForMember(dest => dest.CommandeId, opt => opt.MapFrom(src => src.CommandeId))
            .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.Article))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Quantite));
        
        CreateMap<Article, ArticleOutputDto>()
            .ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
            .ForMember(dest => dest.Libelle, opt => opt.MapFrom(src => src.Libelle))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Reference));

        CreateMap<Famille, FamilleOutputDto>()
            .ForMember(dest => dest.FamilleId, opt => opt.MapFrom(src => src.FamilleId))
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Nom))
            .ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.Articles));

   
        CreateMap<Adresse, AdresseDto>();
        CreateMap<FamilleCreateInputDto, Famille>();
        
        CreateMap<RoleDto, Role>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));
        
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));

        CreateMap<Stock, StockHistoryDto>();
        
        CreateMap<Inventorier, StockHistoryDto>();

        CreateMap<Stock, StockDetailDto>();

        CreateMap<Stock, StockSummaryDto>();

        CreateMap<Stock, StockInputDto>();
    }
}
