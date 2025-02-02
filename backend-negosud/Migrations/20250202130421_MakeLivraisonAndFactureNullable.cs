using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class MakeLivraisonAndFactureNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "commande");

            migrationBuilder.AlterColumn<int>(
                name: "livraison_id",
                table: "commande",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "livraison_id",
                table: "commande",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "commande",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
