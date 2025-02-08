using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContraintesCommande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ligne_commande_article1_fk",
                table: "ligne_commande");

            migrationBuilder.DropForeignKey(
                name: "ligne_commande_commande0_fk",
                table: "ligne_commande");

            migrationBuilder.AddForeignKey(
                name: "ligne_commande_article1_fk",
                table: "ligne_commande",
                column: "article_id",
                principalTable: "article",
                principalColumn: "article_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ligne_commande_commande0_fk",
                table: "ligne_commande",
                column: "commande_id",
                principalTable: "commande",
                principalColumn: "commande_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ligne_commande_article1_fk",
                table: "ligne_commande");

            migrationBuilder.DropForeignKey(
                name: "ligne_commande_commande0_fk",
                table: "ligne_commande");

            migrationBuilder.AddForeignKey(
                name: "ligne_commande_article1_fk",
                table: "ligne_commande",
                column: "article_id",
                principalTable: "article",
                principalColumn: "article_id");

            migrationBuilder.AddForeignKey(
                name: "ligne_commande_commande0_fk",
                table: "ligne_commande",
                column: "commande_id",
                principalTable: "commande",
                principalColumn: "commande_id");
        }
    }
}
