
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class DeleteQuantiteInCommande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantite",
                table: "Commande");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quantite",
                table: "Commande",
                type: "int",
                nullable: false,
                defaultValue: 0); 
        }
    }
}
