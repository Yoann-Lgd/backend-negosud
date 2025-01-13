using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.entities;

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

    public virtual DbSet<LigneBonCommande> LigneBonCommandes { get; set; }

    public virtual DbSet<LigneCommande> LigneCommandes { get; set; }

    public virtual DbSet<LigneLivraison> LigneLivraisons { get; set; }

    public virtual DbSet<Livraison> Livraisons { get; set; }

    public virtual DbSet<Pay> Pays { get; set; }

    public virtual DbSet<Reglement> Reglements { get; set; }

    public virtual DbSet<ReinitialisationMdp> ReinitialisationMdps { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Tva> Tvas { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost:5432;Username=postgres;Password=nego69;Database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adresse>(entity =>
        {
            entity.HasKey(e => e.AdresseId).HasName("adresse_pkey");

            entity.ToTable("adresse");

            entity.Property(e => e.AdresseId).HasColumnName("adresse_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CodePostal).HasColumnName("code_postal");
            entity.Property(e => e.Departement)
                .HasMaxLength(50)
                .HasColumnName("departement");
            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.PaysId).HasColumnName("pays_id");
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.Ville)
                .HasColumnType("character varying")
                .HasColumnName("ville");

            entity.HasOne(d => d.Client).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_client_id_fkey");

            entity.HasOne(d => d.Fournisseur).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.FournisseurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_fournisseur_id_fkey");

            entity.HasOne(d => d.Pays).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.PaysId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_pays_id_fkey");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_utilisateur_id_fkey");

            entity.HasMany(d => d.Livraisons).WithMany(p => p.Adresses)
                .UsingEntity<Dictionary<string, object>>(
                    "Lier",
                    r => r.HasOne<Livraison>().WithMany()
                        .HasForeignKey("LivraisonId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lier_livraison_id_fkey"),
                    l => l.HasOne<Adresse>().WithMany()
                        .HasForeignKey("AdresseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lier_adresse_id_fkey"),
                    j =>
                    {
                        j.HasKey("AdresseId", "LivraisonId").HasName("lier_pkey");
                        j.ToTable("lier");
                        j.IndexerProperty<int>("AdresseId").HasColumnName("adresse_id");
                        j.IndexerProperty<int>("LivraisonId").HasColumnName("livraison_id");
                    });
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("article_pkey");

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
                .HasConstraintName("article_famille_id_fkey");

            entity.HasOne(d => d.Fournisseur).WithMany(p => p.Articles)
                .HasForeignKey(d => d.FournisseurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("article_fournisseur_id_fkey");

            entity.HasOne(d => d.Tva).WithMany(p => p.Articles)
                .HasForeignKey(d => d.TvaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("article_tva_id_fkey");
        });

        modelBuilder.Entity<BonCommande>(entity =>
        {
            entity.HasKey(e => e.BonCommandeId).HasName("bon_commande_pkey");

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
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.BonCommandes)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bon_commande_utilisateur_id_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
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
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.CommandeId).HasName("commande_pkey");

            entity.ToTable("commande");

            entity.HasIndex(e => e.FactureId, "commande_facture_id_key").IsUnique();

            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateCreation)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_creation");
            entity.Property(e => e.FactureId).HasColumnName("facture_id");
            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.Valide).HasColumnName("valide");

            entity.HasOne(d => d.Client).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("commande_client_id_fkey");

            entity.HasOne(d => d.Facture).WithOne(p => p.Commande)
                .HasForeignKey<Commande>(d => d.FactureId)
                .HasConstraintName("commande_facture_id_fkey");

            entity.HasOne(d => d.Livraison).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.LivraisonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("commande_livraison_id_fkey");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.FactureId).HasName("facture_pkey");

            entity.ToTable("facture");

            entity.HasIndex(e => e.CommandeId, "facture_commande_id_key").IsUnique();

            entity.Property(e => e.FactureId).HasColumnName("facture_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.DateFacturation)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_facturation");
            entity.Property(e => e.MontantHt).HasColumnName("montant_ht");
            entity.Property(e => e.MontantTtc).HasColumnName("montant_ttc");
            entity.Property(e => e.MontantTva).HasColumnName("montant_tva");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");

            entity.HasOne(d => d.Client).WithMany(p => p.Factures)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("facture_client_id_fkey");

            entity.HasOne(d => d.CommandeNavigation).WithOne(p => p.FactureNavigation)
                .HasForeignKey<Facture>(d => d.CommandeId)
                .HasConstraintName("facture_commande_id_fkey");
        });

        modelBuilder.Entity<Famille>(entity =>
        {
            entity.HasKey(e => e.FamilleId).HasName("famille_pkey");

            entity.ToTable("famille");

            entity.Property(e => e.FamilleId).HasColumnName("famille_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Fournisseur>(entity =>
        {
            entity.HasKey(e => e.FournisseurId).HasName("fournisseur_pkey");

            entity.ToTable("fournisseur");

            entity.Property(e => e.FournisseurId).HasColumnName("fournisseur_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.RaisonSociale)
                .HasMaxLength(150)
                .HasColumnName("raison_sociale");
            entity.Property(e => e.Tel)
                .HasMaxLength(15)
                .HasColumnName("tel");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("image_pkey");

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
                .HasConstraintName("image_article_id_fkey");
        });

        modelBuilder.Entity<Inventorier>(entity =>
        {
            entity.HasKey(e => new { e.UtilisateurId, e.StockId }).HasName("inventorier_pkey");

            entity.ToTable("inventorier");

            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.DateModification)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_modification");
            entity.Property(e => e.QuantitePostModification).HasColumnName("quantite_post_modification");
            entity.Property(e => e.QuantitePrecedente).HasColumnName("quantite_precedente");
            entity.Property(e => e.TypeModification)
                .HasMaxLength(70)
                .HasColumnName("type_modification");

            entity.HasOne(d => d.Stock).WithMany(p => p.Inventoriers)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventorier_stock_id_fkey");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Inventoriers)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventorier_utilisateur_id_fkey");
        });

        modelBuilder.Entity<LigneBonCommande>(entity =>
        {
            entity.HasKey(e => e.LigneBonCommandeId).HasName("ligne_bon_commande_pkey");

            entity.ToTable("ligne_bon_commande");

            entity.Property(e => e.LigneBonCommandeId).HasColumnName("ligne_bon_commande_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.BonCommandeId).HasColumnName("bon_commande_id");
            entity.Property(e => e.LigneLivraisonId).HasColumnName("ligne_livraison_id");
            entity.Property(e => e.PrixUnitaire).HasColumnName("prix_unitaire");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.Article).WithMany(p => p.LigneBonCommandes)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_bon_commande_article_id_fkey");

            entity.HasOne(d => d.BonCommande).WithMany(p => p.LigneBonCommandes)
                .HasForeignKey(d => d.BonCommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_bon_commande_bon_commande_id_fkey");

            entity.HasOne(d => d.LigneLivraison).WithMany(p => p.LigneBonCommandes)
                .HasForeignKey(d => d.LigneLivraisonId)
                .HasConstraintName("ligne_bon_commande_ligne_livraison_id_fkey");
        });

        modelBuilder.Entity<LigneCommande>(entity =>
        {
            entity.HasKey(e => e.LigneCommandeId).HasName("ligne_commande_pkey");

            entity.ToTable("ligne_commande");

            entity.Property(e => e.LigneCommandeId).HasColumnName("ligne_commande_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.Article).WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_commande_article_id_fkey");

            entity.HasOne(d => d.Commande).WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.CommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_commande_commande_id_fkey");
        });

        modelBuilder.Entity<LigneLivraison>(entity =>
        {
            entity.HasKey(e => e.LigneLivraisonId).HasName("ligne_livraison_pkey");

            entity.ToTable("ligne_livraison");

            entity.Property(e => e.LigneLivraisonId).HasColumnName("ligne_livraison_id");
            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.Livraison).WithMany(p => p.LigneLivraisons)
                .HasForeignKey(d => d.LivraisonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ligne_livraison_livraison_id_fkey");
        });

        modelBuilder.Entity<Livraison>(entity =>
        {
            entity.HasKey(e => e.LivraisonId).HasName("livraison_pkey");

            entity.ToTable("livraison");

            entity.Property(e => e.LivraisonId).HasColumnName("livraison_id");
            entity.Property(e => e.DateEstimee)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_estimee");
            entity.Property(e => e.DateLivraison)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_livraison");
            entity.Property(e => e.Livree).HasColumnName("livree");
        });

        modelBuilder.Entity<Pay>(entity =>
        {
            entity.HasKey(e => e.PaysId).HasName("pays_pkey");

            entity.ToTable("pays");

            entity.Property(e => e.PaysId).HasColumnName("pays_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Reglement>(entity =>
        {
            entity.HasKey(e => e.ReglementId).HasName("reglement_pkey");

            entity.ToTable("reglement");

            entity.Property(e => e.ReglementId).HasColumnName("reglement_id");
            entity.Property(e => e.CommandeId).HasColumnName("commande_id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Montant).HasColumnName("montant");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");

            entity.HasOne(d => d.Commande).WithMany(p => p.Reglements)
                .HasForeignKey(d => d.CommandeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reglement_commande_id_fkey");
        });

        modelBuilder.Entity<ReinitialisationMdp>(entity =>
        {
            entity.HasKey(e => e.ReinitialisationMdpId).HasName("reinitialisation_mdp_pkey");

            entity.ToTable("reinitialisation_mdp");

            entity.Property(e => e.ReinitialisationMdpId).HasColumnName("reinitialisation_mdp_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateDemande)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_demande");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(300)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(300)
                .HasColumnName("reset_token");
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");

            entity.HasOne(d => d.Client).WithMany(p => p.ReinitialisationMdps)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reinitialisation_mdp_client_id_fkey");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.ReinitialisationMdps)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reinitialisation_mdp_utilisateur_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Nom)
                .HasMaxLength(70)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("stock_pkey");

            entity.ToTable("stock");

            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.ReapprovisionnementAuto).HasColumnName("reapprovisionnement_auto");
            entity.Property(e => e.RefLot)
                .HasMaxLength(50)
                .HasColumnName("ref_lot");
            entity.Property(e => e.SeuilMinimum).HasColumnName("seuil_minimum");

            entity.HasOne(d => d.Article).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_article_id_fkey");
        });

        modelBuilder.Entity<Tva>(entity =>
        {
            entity.HasKey(e => e.TvaId).HasName("tva_pkey");

            entity.ToTable("tva");

            entity.Property(e => e.TvaId).HasColumnName("tva_id");
            entity.Property(e => e.Valeur).HasColumnName("valeur");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurId).HasName("utilisateur_pkey");

            entity.ToTable("utilisateur");

            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(300)
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
                .HasMaxLength(15)
                .HasColumnName("telephone");

            entity.HasOne(d => d.Role).WithMany(p => p.Utilisateurs)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("utilisateur_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
