using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    tel = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    est_valide = table.Column<bool>(type: "boolean", nullable: false),
                    acess_token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("client_pk", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "famille",
                columns: table => new
                {
                    famille_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("famille_pk", x => x.famille_id);
                });

            migrationBuilder.CreateTable(
                name: "fournisseur",
                columns: table => new
                {
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    raison_sociale = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tel = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fournisseur_pk", x => x.fournisseur_id);
                });

            migrationBuilder.CreateTable(
                name: "livraison",
                columns: table => new
                {
                    livraison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_estimee = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_livraison = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    livree = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("livraison_pk", x => x.livraison_id);
                });

            migrationBuilder.CreateTable(
                name: "pays",
                columns: table => new
                {
                    pays_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pays_pk", x => x.pays_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_pk", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "tva",
                columns: table => new
                {
                    tva_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valeur = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tva_pk", x => x.tva_id);
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
                    table.PrimaryKey("lier_pk", x => new { x.adresse_id, x.livraison_id });
                    table.ForeignKey(
                        name: "lier_livraison1_fk",
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
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    telephone = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    access_token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("utilisateur_pk", x => x.utilisateur_id);
                    table.ForeignKey(
                        name: "utilisateur_role0_fk",
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
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    reference = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    prix = table.Column<double>(type: "double precision", nullable: false),
                    famille_id = table.Column<int>(type: "integer", nullable: false),
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false),
                    tva_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_pk", x => x.article_id);
                    table.ForeignKey(
                        name: "article_famille0_fk",
                        column: x => x.famille_id,
                        principalTable: "famille",
                        principalColumn: "famille_id");
                    table.ForeignKey(
                        name: "article_fournisseur1_fk",
                        column: x => x.fournisseur_id,
                        principalTable: "fournisseur",
                        principalColumn: "fournisseur_id");
                    table.ForeignKey(
                        name: "article_tva2_fk",
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
                    numero = table.Column<int>(type: "integer", nullable: false),
                    ville = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code_postal = table.Column<int>(type: "integer", nullable: false),
                    departement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pays_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: true),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: true),
                    fournisseur_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("adresse_pk", x => x.adresse_id);
                    table.ForeignKey(
                        name: "adresse_client1_fk",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "adresse_fournisseur3_fk",
                        column: x => x.fournisseur_id,
                        principalTable: "fournisseur",
                        principalColumn: "fournisseur_id");
                    table.ForeignKey(
                        name: "adresse_pays0_fk",
                        column: x => x.pays_id,
                        principalTable: "pays",
                        principalColumn: "pays_id");
                    table.ForeignKey(
                        name: "adresse_utilisateur2_fk",
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
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reference = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    prix = table.Column<double>(type: "double precision", precision: 15, scale: 3, nullable: false),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    fournisseur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bon_commande_pk", x => x.bon_commande_id);
                    table.ForeignKey(
                        name: "bon_commande_fournisseur1_fk",
                        column: x => x.fournisseur_id,
                        principalTable: "fournisseur",
                        principalColumn: "fournisseur_id");
                    table.ForeignKey(
                        name: "bon_commande_utilisateur0_fk",
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
                    date_demande = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mot_de_passe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    reset_token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reinitialisation_mdp_pk", x => x.reinitialisation_mdp_id);
                    table.ForeignKey(
                        name: "reinitialisation_mdp_client1_fk",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "reinitialisation_mdp_utilisateur0_fk",
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
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("image_pk", x => x.image_id);
                    table.ForeignKey(
                        name: "image_article0_fk",
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
                    ref_lot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    seuil_minimum = table.Column<int>(type: "integer", nullable: false),
                    reapprovisionnement_auto = table.Column<bool>(type: "boolean", nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stock_pk", x => x.stock_id);
                    table.ForeignKey(
                        name: "stock_article0_fk",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
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
                    livree = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ligne_bon_commande_pk", x => x.ligne_bon_commande_id);
                    table.ForeignKey(
                        name: "ligne_bon_commande_article0_fk",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id");
                    table.ForeignKey(
                        name: "ligne_bon_commande_bon_commande1_fk",
                        column: x => x.bon_commande_id,
                        principalTable: "bon_commande",
                        principalColumn: "bon_commande_id");
                });

            migrationBuilder.CreateTable(
                name: "inventorier",
                columns: table => new
                {
                    utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    stock_id = table.Column<int>(type: "integer", nullable: false),
                    date_modification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type_modification = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    quantite_precedente = table.Column<int>(type: "integer", nullable: false),
                    quantite_post_modification = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("inventorier_pk", x => new { x.utilisateur_id, x.stock_id });
                    table.ForeignKey(
                        name: "inventorier_stock1_fk",
                        column: x => x.stock_id,
                        principalTable: "stock",
                        principalColumn: "stock_id");
                    table.ForeignKey(
                        name: "inventorier_utilisateur0_fk",
                        column: x => x.utilisateur_id,
                        principalTable: "utilisateur",
                        principalColumn: "utilisateur_id");
                });

            migrationBuilder.CreateTable(
                name: "ligne_livraison",
                columns: table => new
                {
                    ligne_livraison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    livraison_id = table.Column<int>(type: "integer", nullable: false),
                    ligne_bon_commande_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ligne_livraison_pk", x => x.ligne_livraison_id);
                    table.ForeignKey(
                        name: "ligne_livraison_ligne_bon_commande1_fk",
                        column: x => x.ligne_bon_commande_id,
                        principalTable: "ligne_bon_commande",
                        principalColumn: "ligne_bon_commande_id");
                    table.ForeignKey(
                        name: "ligne_livraison_livraison0_fk",
                        column: x => x.livraison_id,
                        principalTable: "livraison",
                        principalColumn: "livraison_id");
                });

            migrationBuilder.CreateTable(
                name: "commande",
                columns: table => new
                {
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valide = table.Column<bool>(type: "boolean", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    livraison_id = table.Column<int>(type: "integer", nullable: true),
                    facture_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("commande_pk", x => x.commande_id);
                    table.ForeignKey(
                        name: "commande_client0_fk",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "commande_livraison1_fk",
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
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_facturation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    montant_ht = table.Column<double>(type: "double precision", nullable: false),
                    montant_ttc = table.Column<double>(type: "double precision", nullable: false),
                    montant_tva = table.Column<double>(type: "double precision", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("facture_pk", x => x.facture_id);
                    table.ForeignKey(
                        name: "facture_client0_fk",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "facture_commande1_fk",
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
                    table.PrimaryKey("ligne_commande_pk", x => x.ligne_commande_id);
                    table.ForeignKey(
                        name: "ligne_commande_article1_fk",
                        column: x => x.article_id,
                        principalTable: "article",
                        principalColumn: "article_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ligne_commande_commande0_fk",
                        column: x => x.commande_id,
                        principalTable: "commande",
                        principalColumn: "commande_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reglement",
                columns: table => new
                {
                    reglement_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    montant = table.Column<double>(type: "double precision", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reglement_pk", x => x.reglement_id);
                    table.ForeignKey(
                        name: "reglement_commande0_fk",
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
                name: "IX_bon_commande_fournisseur_id",
                table: "bon_commande",
                column: "fournisseur_id");

            migrationBuilder.CreateIndex(
                name: "IX_bon_commande_utilisateur_id",
                table: "bon_commande",
                column: "utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "commande_facture0_ak",
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
                name: "facture_commande0_ak",
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
                name: "ligne_livraison_ligne_bon_commande0_ak",
                table: "ligne_livraison",
                column: "ligne_bon_commande_id",
                unique: true);

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
                name: "commande_facture2_fk",
                table: "commande",
                column: "facture_id",
                principalTable: "facture",
                principalColumn: "facture_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "commande_client0_fk",
                table: "commande");

            migrationBuilder.DropForeignKey(
                name: "facture_client0_fk",
                table: "facture");

            migrationBuilder.DropForeignKey(
                name: "commande_facture2_fk",
                table: "commande");

            migrationBuilder.DropTable(
                name: "adresse");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "inventorier");

            migrationBuilder.DropTable(
                name: "lier");

            migrationBuilder.DropTable(
                name: "ligne_commande");

            migrationBuilder.DropTable(
                name: "ligne_livraison");

            migrationBuilder.DropTable(
                name: "reglement");

            migrationBuilder.DropTable(
                name: "reinitialisation_mdp");

            migrationBuilder.DropTable(
                name: "pays");

            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "ligne_bon_commande");

            migrationBuilder.DropTable(
                name: "article");

            migrationBuilder.DropTable(
                name: "bon_commande");

            migrationBuilder.DropTable(
                name: "famille");

            migrationBuilder.DropTable(
                name: "tva");

            migrationBuilder.DropTable(
                name: "fournisseur");

            migrationBuilder.DropTable(
                name: "utilisateur");

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
