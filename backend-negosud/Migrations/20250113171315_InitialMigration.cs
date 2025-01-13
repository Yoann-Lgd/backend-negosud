using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    tel = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    est_valide = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("client_pkey", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "famille",
                columns: table => new
                {
                    famille_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("famille_pkey", x => x.famille_id);
                });

            migrationBuilder.CreateTable(
                name: "fournisseur",
                columns: table => new
                {
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    raison_sociale = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tel = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fournisseur_pkey", x => x.fournisseur_id);
                });

            migrationBuilder.CreateTable(
                name: "livraison",
                columns: table => new
                {
                    livraison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_estimee = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_livraison = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    livree = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("livraison_pkey", x => x.livraison_id);
                });

            migrationBuilder.CreateTable(
                name: "pays",
                columns: table => new
                {
                    pays_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pays_pkey", x => x.pays_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_pkey", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "tva",
                columns: table => new
                {
                    tva_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valeur = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tva_pkey", x => x.tva_id);
                });

            migrationBuilder.CreateTable(
                name: "ligne_livraison",
                columns: table => new
                {
                    ligne_livraison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    livraison_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ligne_livraison_pkey", x => x.ligne_livraison_id);
                    table.ForeignKey(
                        name: "ligne_livraison_livraison_id_fkey",
                        column: x => x.livraison_id,
                        principalTable: "livraison",
                        principalColumn: "livraison_id");
                });

            migrationBuilder.CreateTable(
                name: "utilisateur",
                columns: table => new
                {
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    telephone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    access_token = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("utilisateur_pkey", x => x.utilisateur_id);
                    table.ForeignKey(
                        name: "utilisateur_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "article",
                columns: table => new
                {
                    article_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    reference = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    prix = table.Column<double>(type: "double precision", nullable: true),
                    famille_id = table.Column<int>(type: "integer", nullable: false),
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false),
                    tva_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_pkey", x => x.article_id);
                    table.ForeignKey(
                        name: "article_famille_id_fkey",
                        column: x => x.famille_id,
                        principalTable: "famille",
                        principalColumn: "famille_id");
                    table.ForeignKey(
                        name: "article_fournisseur_id_fkey",
                        column: x => x.fournisseur_id,
                        principalTable: "fournisseur",
                        principalColumn: "fournisseur_id");
                    table.ForeignKey(
                        name: "article_tva_id_fkey",
                        column: x => x.tva_id,
                        principalTable: "tva",
                        principalColumn: "tva_id");
                });

            migrationBuilder.CreateTable(
                name: "adresse",
                columns: table => new
                {
                    adresse_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero = table.Column<int>(type: "integer", nullable: true),
                    ville = table.Column<string>(type: "character varying", nullable: true),
                    code_postal = table.Column<int>(type: "integer", nullable: true),
                    departement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    pays_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("adresse_pkey", x => x.adresse_id);
                    table.ForeignKey(
                        name: "adresse_client_id_fkey",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "adresse_fournisseur_id_fkey",
                        column: x => x.fournisseur_id,
                        principalTable: "fournisseur",
                        principalColumn: "fournisseur_id");
                    table.ForeignKey(
                        name: "adresse_pays_id_fkey",
                        column: x => x.pays_id,
                        principalTable: "pays",
                        principalColumn: "pays_id");
                    table.ForeignKey(
                        name: "adresse_utilisateur_id_fkey",
                        column: x => x.utilisateur_id,
                        principalTable: "utilisateur",
                        principalColumn: "utilisateur_id");
                });

            migrationBuilder.CreateTable(
                name: "bon_commande",
                columns: table => new
                {
                    bon_commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    reference = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    prix = table.Column<decimal>(type: "numeric(15,3)", precision: 15, scale: 3, nullable: true),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bon_commande_pkey", x => x.bon_commande_id);
                    table.ForeignKey(
                        name: "bon_commande_utilisateur_id_fkey",
                        column: x => x.utilisateur_id,
                        principalTable: "utilisateur",
                        principalColumn: "utilisateur_id");
                });

            migrationBuilder.CreateTable(
                name: "reinitialisation_mdp",
                columns: table => new
                {
                    reinitialisation_mdp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_demande = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    reset_token = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reinitialisation_mdp_pkey", x => x.reinitialisation_mdp_id);
                    table.ForeignKey(
                        name: "reinitialisation_mdp_client_id_fkey",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "reinitialisation_mdp_utilisateur_id_fkey",
                        column: x => x.utilisateur_id,
                        principalTable: "utilisateur",
                        principalColumn: "utilisateur_id");
                });

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    article_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("image_pkey", x => x.image_id);
                    table.ForeignKey(
                        name: "image_article_id_fkey",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
                });

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    stock_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ref_lot = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    quantite = table.Column<int>(type: "integer", nullable: true),
                    seuil_minimum = table.Column<double>(type: "double precision", nullable: true),
                    reapprovisionnement_auto = table.Column<bool>(type: "boolean", nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stock_pkey", x => x.stock_id);
                    table.ForeignKey(
                        name: "stock_article_id_fkey",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
                });

            migrationBuilder.CreateTable(
                name: "lier",
                columns: table => new
                {
                    adresse_id = table.Column<int>(type: "integer", nullable: false),
                    livraison_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lier_pkey", x => new { x.adresse_id, x.livraison_id });
                    table.ForeignKey(
                        name: "lier_adresse_id_fkey",
                        column: x => x.adresse_id,
                        principalTable: "adresse",
                        principalColumn: "adresse_id");
                    table.ForeignKey(
                        name: "lier_livraison_id_fkey",
                        column: x => x.livraison_id,
                        principalTable: "livraison",
                        principalColumn: "livraison_id");
                });

            migrationBuilder.CreateTable(
                name: "ligne_bon_commande",
                columns: table => new
                {
                    ligne_bon_commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    prix_unitaire = table.Column<double>(type: "double precision", nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false),
                    bon_commande_id = table.Column<int>(type: "integer", nullable: false),
                    ligne_livraison_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ligne_bon_commande_pkey", x => x.ligne_bon_commande_id);
                    table.ForeignKey(
                        name: "ligne_bon_commande_article_id_fkey",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
                    table.ForeignKey(
                        name: "ligne_bon_commande_bon_commande_id_fkey",
                        column: x => x.bon_commande_id,
                        principalTable: "bon_commande",
                        principalColumn: "bon_commande_id");
                    table.ForeignKey(
                        name: "ligne_bon_commande_ligne_livraison_id_fkey",
                        column: x => x.ligne_livraison_id,
                        principalTable: "ligne_livraison",
                        principalColumn: "ligne_livraison_id");
                });

            migrationBuilder.CreateTable(
                name: "inventorier",
                columns: table => new
                {
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    stock_id = table.Column<int>(type: "integer", nullable: false),
                    date_modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    type_modification = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    quantite_precedente = table.Column<int>(type: "integer", nullable: false),
                    quantite_post_modification = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("inventorier_pkey", x => new { x.utilisateur_id, x.stock_id });
                    table.ForeignKey(
                        name: "inventorier_stock_id_fkey",
                        column: x => x.stock_id,
                        principalTable: "stock",
                        principalColumn: "stock_id");
                    table.ForeignKey(
                        name: "inventorier_utilisateur_id_fkey",
                        column: x => x.utilisateur_id,
                        principalTable: "utilisateur",
                        principalColumn: "utilisateur_id");
                });

            migrationBuilder.CreateTable(
                name: "commande",
                columns: table => new
                {
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    valide = table.Column<bool>(type: "boolean", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    livraison_id = table.Column<int>(type: "integer", nullable: false),
                    facture_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("commande_pkey", x => x.commande_id);
                    table.ForeignKey(
                        name: "commande_client_id_fkey",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "commande_livraison_id_fkey",
                        column: x => x.livraison_id,
                        principalTable: "livraison",
                        principalColumn: "livraison_id");
                });

            migrationBuilder.CreateTable(
                name: "facture",
                columns: table => new
                {
                    facture_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    date_facturation = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    montant_ht = table.Column<double>(type: "double precision", nullable: false),
                    montant_ttc = table.Column<double>(type: "double precision", nullable: false),
                    montant_tva = table.Column<double>(type: "double precision", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    commande_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("facture_pkey", x => x.facture_id);
                    table.ForeignKey(
                        name: "facture_client_id_fkey",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "facture_commande_id_fkey",
                        column: x => x.commande_id,
                        principalTable: "commande",
                        principalColumn: "commande_id");
                });

            migrationBuilder.CreateTable(
                name: "ligne_commande",
                columns: table => new
                {
                    ligne_commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    commande_id = table.Column<int>(type: "integer", nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ligne_commande_pkey", x => x.ligne_commande_id);
                    table.ForeignKey(
                        name: "ligne_commande_article_id_fkey",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
                    table.ForeignKey(
                        name: "ligne_commande_commande_id_fkey",
                        column: x => x.commande_id,
                        principalTable: "commande",
                        principalColumn: "commande_id");
                });

            migrationBuilder.CreateTable(
                name: "reglement",
                columns: table => new
                {
                    reglement_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    montant = table.Column<double>(type: "double precision", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reglement_pkey", x => x.reglement_id);
                    table.ForeignKey(
                        name: "reglement_commande_id_fkey",
                        column: x => x.commande_id,
                        principalTable: "commande",
                        principalColumn: "commande_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_adresse_client_id",
                table: "adresse",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_adresse_fournisseur_id",
                table: "adresse",
                column: "fournisseur_id");

            migrationBuilder.CreateIndex(
                name: "IX_adresse_pays_id",
                table: "adresse",
                column: "pays_id");

            migrationBuilder.CreateIndex(
                name: "IX_adresse_utilisateur_id",
                table: "adresse",
                column: "utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_famille_id",
                table: "article",
                column: "famille_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_fournisseur_id",
                table: "article",
                column: "fournisseur_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_tva_id",
                table: "article",
                column: "tva_id");

            migrationBuilder.CreateIndex(
                name: "IX_bon_commande_utilisateur_id",
                table: "bon_commande",
                column: "utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "commande_facture_id_key",
                table: "commande",
                column: "facture_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_commande_client_id",
                table: "commande",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_commande_livraison_id",
                table: "commande",
                column: "livraison_id");

            migrationBuilder.CreateIndex(
                name: "facture_commande_id_key",
                table: "facture",
                column: "commande_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facture_client_id",
                table: "facture",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_article_id",
                table: "image",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventorier_stock_id",
                table: "inventorier",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_lier_livraison_id",
                table: "lier",
                column: "livraison_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_bon_commande_article_id",
                table: "ligne_bon_commande",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_bon_commande_bon_commande_id",
                table: "ligne_bon_commande",
                column: "bon_commande_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_bon_commande_ligne_livraison_id",
                table: "ligne_bon_commande",
                column: "ligne_livraison_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_commande_article_id",
                table: "ligne_commande",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_commande_commande_id",
                table: "ligne_commande",
                column: "commande_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_livraison_livraison_id",
                table: "ligne_livraison",
                column: "livraison_id");

            migrationBuilder.CreateIndex(
                name: "IX_reglement_commande_id",
                table: "reglement",
                column: "commande_id");

            migrationBuilder.CreateIndex(
                name: "IX_reinitialisation_mdp_client_id",
                table: "reinitialisation_mdp",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_reinitialisation_mdp_utilisateur_id",
                table: "reinitialisation_mdp",
                column: "utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_article_id",
                table: "stock",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateur_role_id",
                table: "utilisateur",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "commande_facture_id_fkey",
                table: "commande",
                column: "facture_id",
                principalTable: "facture",
                principalColumn: "facture_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "commande_client_id_fkey",
                table: "commande");

            migrationBuilder.DropForeignKey(
                name: "facture_client_id_fkey",
                table: "facture");

            migrationBuilder.DropForeignKey(
                name: "commande_facture_id_fkey",
                table: "commande");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "inventorier");

            migrationBuilder.DropTable(
                name: "lier");

            migrationBuilder.DropTable(
                name: "ligne_bon_commande");

            migrationBuilder.DropTable(
                name: "ligne_commande");

            migrationBuilder.DropTable(
                name: "reglement");

            migrationBuilder.DropTable(
                name: "reinitialisation_mdp");

            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "adresse");

            migrationBuilder.DropTable(
                name: "bon_commande");

            migrationBuilder.DropTable(
                name: "ligne_livraison");

            migrationBuilder.DropTable(
                name: "article");

            migrationBuilder.DropTable(
                name: "pays");

            migrationBuilder.DropTable(
                name: "utilisateur");

            migrationBuilder.DropTable(
                name: "famille");

            migrationBuilder.DropTable(
                name: "fournisseur");

            migrationBuilder.DropTable(
                name: "tva");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "facture");

            migrationBuilder.DropTable(
                name: "commande");

            migrationBuilder.DropTable(
                name: "livraison");
        }
    }
}
