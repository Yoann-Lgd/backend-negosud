using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Entities;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adresse> Adresses { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<BonCommande> BonCommandes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Famille> Familles { get; set; }

    public virtual DbSet<Fournisseur> Fournisseurs { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Inventorier> Inventoriers { get; set; }

    public virtual DbSet<Lier> Liers { get; set; }

    public virtual DbSet<LigneBonCommande> LigneBonCommandes { get; set; }

    public virtual DbSet<LigneCommande> LigneCommandes { get; set; }

    public virtual DbSet<LigneLivraison> LigneLivraisons { get; set; }

    public virtual DbSet<Livraison> Livraisons { get; set; }

    public virtual DbSet<Pays> Pays { get; set; }

    public virtual DbSet<Reglement> Reglements { get; set; }

    public virtual DbSet<ReinitialisationMdp> ReinitialisationMdps { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Tva> Tvas { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityByDefaultColumns();
        modelBuilder.Entity<Adresse>(entity =>
        {
            entity.HasKey(e => e.AdresseId).HasName("adresse_pk");

            entity.ToTable("adresse");

            entity.Property(e => e.AdresseId).HasColumnName("adresse_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CodePostal).HasColumnName("code_postal");
            entity.Property(e => e.Departement)
                .HasMaxLength(100)
                .HasColumnName("departement");
            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.PaysId).HasColumnName("pays_id");
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.Ville)
                .HasMaxLength(100)
                .HasColumnName("ville");

            entity.HasOne(d => d.Client).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("adresse_client1_fk");

            entity.HasOne(d => d.Fournisseur).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.FournisseurId)
                .HasConstraintName("adresse_fournisseur3_fk");

            entity.HasOne(d => d.Pays).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.PaysId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_pays0_fk");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.UtilisateurId)
                .HasConstraintName("adresse_utilisateur2_fk");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("article_pk");

            entity.ToTable("article");

            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.FamilleId).HasColumnName("famille_id");
            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");
            entity.Property(e => e.Libelle)
                .HasMaxLength(150)
                .HasColumnName("libelle");
            entity.Property(e => e.Prix).HasColumnName("prix");
            entity.Property(e => e.Reference)
                .HasMaxLength(25)
                .HasColumnName("reference");
            entity.Property(e => e.TvaId).HasColumnName("tva_id");

            entity.HasOne(d => d.Famille).WithMany(p => p.Articles)
                .HasForeignKey(d => d.FamilleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("article_famille0_fk");

            entity.HasOne(d => d.Fournisseur).WithMany(p => p.Articles)
                .HasForeignKey(d => d.FournisseurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("article_fournisseur1_fk");

            entity.HasOne(d => d.Tva).WithMany(p => p.Articles)
                .HasForeignKey(d => d.TvaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("article_tva2_fk");
        });
        
        modelBuilder.Entity<BonCommande>(entity =>
        {
            entity.HasKey(e => e.BonCommandeId).HasName("bon_commande_pk");

            entity.ToTable("bon_commande");

            entity.Property(e => e.BonCommandeId).HasColumnName("bon_commande_id");
            entity.Property(e => e.Prix)
                .HasPrecision(15, 3)
                .HasColumnName("prix");
            entity.Property(e => e.Reference)
                .HasMaxLength(25)
                .HasColumnName("reference");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            
            entity.Property(e => e.DateCreation)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_creation");  
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.BonCommandes)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bon_commande_utilisateur0_fk");
            
            entity.HasOne(d => d.Fournisseur).WithMany(p => p.BonCommandes)
                .HasForeignKey(d => d.FournisseurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bon_commande_fournisseur1_fk");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("client_pk");

            entity.ToTable("client");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.AcessToken)
                .HasMaxLength(1000)
                .HasColumnName("acess_token");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EstValide).HasColumnName("est_valide");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(300)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.Tel)
                .HasMaxLength(15)
                .HasColumnName("tel");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.CommandeId).HasName("commande_pk");

            entity.ToTable("commande");

            entity.HasIndex(e => e.FactureId, "commande_facture0_ak").IsUnique();

            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateCreation)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_creation");                    
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("DeletedAt");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_expiration");
            entity.Property(e => e.FactureId).HasColumnName("facture_id");
            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.Valide).HasColumnName("valide");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Client).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("commande_client0_fk");

            entity.HasOne(d => d.Facture).WithOne(p => p.Commande)
                .HasForeignKey<Commande>(d => d.FactureId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("commande_facture2_fk");

            entity.HasOne(d => d.Livraison).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.LivraisonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("commande_livraison1_fk");
            
            entity.HasMany(c => c.LigneCommandes)
                .WithOne(lc => lc.Commande)
                .HasForeignKey(lc => lc.CommandeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.FactureId).HasName("facture_pk");

            entity.ToTable("facture");

            entity.HasIndex(e => e.CommandeId, "facture_commande0_ak").IsUnique();

            entity.Property(e => e.FactureId).HasColumnName("facture_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.DateFacturation)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_facturation");
            entity.Property(e => e.MontantHt).HasColumnName("montant_ht");
            entity.Property(e => e.MontantTtc).HasColumnName("montant_ttc");
            entity.Property(e => e.MontantTva).HasColumnName("montant_tva");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasColumnName("reference");

            entity.HasOne(d => d.Client).WithMany(p => p.Factures)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("facture_client0_fk");

            entity.HasOne(d => d.CommandeNavigation).WithOne(p => p.FactureNavigation)
                .HasForeignKey<Facture>(d => d.CommandeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("facture_commande1_fk");
        });

        modelBuilder.Entity<Famille>(entity =>
        {
            entity.HasKey(e => e.FamilleId).HasName("famille_pk");

            entity.ToTable("famille");

            entity.Property(e => e.FamilleId).HasColumnName("famille_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Fournisseur>(entity =>
        {
            entity.HasKey(e => e.FournisseurId).HasName("fournisseur_pk");

            entity.ToTable("fournisseur");

            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nom)
                .HasMaxLength(70)
                .HasColumnName("nom");
            entity.Property(e => e.RaisonSociale)
                .HasMaxLength(150)
                .HasColumnName("raison_sociale");
            entity.Property(e => e.Tel)
                .HasMaxLength(25)
                .HasColumnName("tel");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("image_pk");

            entity.ToTable("image");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Format)
                .HasMaxLength(10)
                .HasColumnName("format");
            entity.Property(e => e.Libelle)
                .HasMaxLength(150)
                .HasColumnName("libelle");
            entity.Property(e => e.Slug)
                .HasMaxLength(300)
                .HasColumnName("slug");

            entity.HasOne(d => d.Article).WithMany(p => p.Images)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("image_article0_fk");
        });

        modelBuilder.Entity<Inventorier>(entity =>
        {
            entity.HasKey(e => new { e.UtilisateurId, e.StockId }).HasName("inventorier_pk");

            entity.ToTable("inventorier");

            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.DateModification)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_modification");
            entity.Property(e => e.QuantitePostModification).HasColumnName("quantite_post_modification");
            entity.Property(e => e.QuantitePrecedente).HasColumnName("quantite_precedente");
            entity.Property(e => e.TypeModification)
                .HasMaxLength(70)
                .HasColumnName("type_modification");

            entity.HasOne(d => d.Stock).WithMany(p => p.Inventoriers)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventorier_stock1_fk");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Inventoriers)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventorier_utilisateur0_fk");
        });

        modelBuilder.Entity<Lier>(entity =>
        {
            entity.HasKey(e => new { e.AdresseId, e.LivraisonId }).HasName("lier_pk");

            entity.ToTable("lier");

            entity.Property(e => e.AdresseId).HasColumnName("adresse_id");
            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");

            entity.HasOne(d => d.Livraison).WithMany(p => p.Liers)
                .HasForeignKey(d => d.LivraisonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lier_livraison1_fk");
        });

        modelBuilder.Entity<LigneBonCommande>(entity =>
        {
            entity.HasKey(e => e.LigneBonCommandeId).HasName("ligne_bon_commande_pk");

            entity.ToTable("ligne_bon_commande");

            entity.Property(e => e.LigneBonCommandeId).HasColumnName("ligne_bon_commande_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.BonCommandeId).HasColumnName("bon_commande_id");
            entity.Property(e => e.PrixUnitaire).HasColumnName("prix_unitaire");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.Livree).HasColumnName("livree");

            entity.HasOne(d => d.Article).WithMany(p => p.LigneBonCommandes)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_bon_commande_article0_fk");

            entity.HasOne(d => d.BonCommande).WithMany(p => p.LigneBonCommandes)
                .HasForeignKey(d => d.BonCommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_bon_commande_bon_commande1_fk");
        });

        modelBuilder.Entity<LigneCommande>(entity =>
        {
            entity.HasKey(e => e.LigneCommandeId).HasName("ligne_commande_pk");

            entity.ToTable("ligne_commande");

            entity.Property(e => e.LigneCommandeId).HasColumnName("ligne_commande_id").ValueGeneratedOnAdd();
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.Article)
                .WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("ligne_commande_article1_fk");

            entity.HasOne(d => d.Commande).WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.CommandeId)
                .OnDelete(DeleteBehavior.Cascade) 
                .HasConstraintName("ligne_commande_commande0_fk");
        });

        modelBuilder.Entity<LigneLivraison>(entity =>
        {
            entity.HasKey(e => e.LigneLivraisonId).HasName("ligne_livraison_pk");

            entity.ToTable("ligne_livraison");

            entity.HasIndex(e => e.LigneBonCommandeId, "ligne_livraison_ligne_bon_commande0_ak").IsUnique();

            entity.Property(e => e.LigneLivraisonId).HasColumnName("ligne_livraison_id");
            entity.Property(e => e.LigneBonCommandeId).HasColumnName("ligne_bon_commande_id");
            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.LigneBonCommande).WithOne(p => p.LigneLivraison)
                .HasForeignKey<LigneLivraison>(d => d.LigneBonCommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_livraison_ligne_bon_commande1_fk");

            entity.HasOne(d => d.Livraison).WithMany(p => p.LigneLivraisons)
                .HasForeignKey(d => d.LivraisonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_livraison_livraison0_fk");
        });

        modelBuilder.Entity<Livraison>(entity =>
        {
            entity.HasKey(e => e.LivraisonId).HasName("livraison_pk");

            entity.ToTable("livraison");

            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.DateEstimee)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_estimee");
            entity.Property(e => e.DateLivraison)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_livraison");
            entity.Property(e => e.Livree).HasColumnName("livree");
        });

        modelBuilder.Entity<Pays>(entity =>
        {
            entity.HasKey(e => e.PaysId).HasName("pays_pk");

            entity.ToTable("pays");

            entity.Property(e => e.PaysId).HasColumnName("pays_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(150)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Reglement>(entity =>
        {
            entity.HasKey(e => e.ReglementId).HasName("reglement_pk");

            entity.ToTable("reglement");

            entity.Property(e => e.ReglementId).HasColumnName("reglement_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date");
            entity.Property(e => e.Montant).HasColumnName("montant");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");

            entity.HasOne(d => d.Commande).WithMany(p => p.Reglements)
                .HasForeignKey(d => d.CommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reglement_commande0_fk");
        });

        modelBuilder.Entity<ReinitialisationMdp>(entity =>
        {
            entity.HasKey(e => e.ReinitialisationMdpId).HasName("reinitialisation_mdp_pk");

            entity.ToTable("reinitialisation_mdp");

            entity.Property(e => e.ReinitialisationMdpId).HasColumnName("reinitialisation_mdp_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateDemande)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("date_demande");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(300)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(1000)
                .HasColumnName("reset_token");
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");

            entity.HasOne(d => d.Client).WithMany(p => p.ReinitialisationMdps)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reinitialisation_mdp_client1_fk");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.ReinitialisationMdps)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reinitialisation_mdp_utilisateur0_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pk");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(70)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("stock_pk");

            entity.ToTable("stock");

            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.ReapprovisionnementAuto).HasColumnName("reapprovisionnement_auto");
            entity.Property(e => e.RefLot)
                .HasMaxLength(100)
                .HasColumnName("ref_lot");
            entity.Property(e => e.SeuilMinimum).HasColumnName("seuil_minimum");

            entity.HasOne(d => d.Article).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_article0_fk");
        });

        modelBuilder.Entity<Tva>(entity =>
        {
            entity.HasKey(e => e.TvaId).HasName("tva_pk");

            entity.ToTable("tva");

            entity.Property(e => e.TvaId).HasColumnName("tva_id");
            entity.Property(e => e.Valeur).HasColumnName("valeur");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurId).HasName("utilisateur_pk");

            entity.ToTable("utilisateur");

            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(1000)
                .HasColumnName("access_token");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(300)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Telephone)
                .HasMaxLength(25)
                .HasColumnName("telephone");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deleted_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Utilisateurs)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("utilisateur_role0_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
