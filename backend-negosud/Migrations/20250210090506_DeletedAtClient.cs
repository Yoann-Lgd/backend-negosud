using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_negosud.Migrations
{
    /// <inheritdoc />
    public partial class DeletedAtClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_creation",
                table: "client",
                newName: "deleted_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "client",
                newName: "date_creation");
        }
    }
}
