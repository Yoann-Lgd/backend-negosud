using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationForBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "commande_client0_fk",
                table: "commande");

            migrationBuilder.DropForeignKey(
                name: "facture_commande1_fk",
                table: "facture");

            migrationBuilder.AddColumn<int>(
                name: "CommandeNavigationCommandeId",
                table: "facture",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "valide",
                table: "commande",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "livraison_id",
                table: "commande",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_creation",
                table: "commande",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "statut",
                table: "commande",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "total",
                table: "commande",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_facture_CommandeNavigationCommandeId",
                table: "facture",
                column: "CommandeNavigationCommandeId");

            migrationBuilder.AddForeignKey(
                name: "commande_client0_fk",
                table: "commande",
                column: "client_id",
                principalTable: "client",
                principalColumn: "client_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_facture_commande_CommandeNavigationCommandeId",
                table: "facture",
                column: "CommandeNavigationCommandeId",
                principalTable: "commande",
                principalColumn: "commande_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "commande_client0_fk",
                table: "commande");

            migrationBuilder.DropForeignKey(
                name: "FK_facture_commande_CommandeNavigationCommandeId",
                table: "facture");

            migrationBuilder.DropIndex(
                name: "IX_facture_CommandeNavigationCommandeId",
                table: "facture");

            migrationBuilder.DropColumn(
                name: "CommandeNavigationCommandeId",
                table: "facture");

            migrationBuilder.DropColumn(
                name: "statut",
                table: "commande");

            migrationBuilder.DropColumn(
                name: "total",
                table: "commande");

            migrationBuilder.AlterColumn<bool>(
                name: "valide",
                table: "commande",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "livraison_id",
                table: "commande",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_creation",
                table: "commande",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddForeignKey(
                name: "commande_client0_fk",
                table: "commande",
                column: "client_id",
                principalTable: "client",
                principalColumn: "client_id");

            migrationBuilder.AddForeignKey(
                name: "facture_commande1_fk",
                table: "facture",
                column: "commande_id",
                principalTable: "commande",
                principalColumn: "commande_id");
        }
    }
}
