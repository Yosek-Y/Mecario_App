using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mecario_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoPieza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigoPieza",
                table: "Piezas",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Piezas_codigoPieza",
                table: "Piezas",
                column: "codigoPieza",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Piezas_codigoPieza",
                table: "Piezas");

            migrationBuilder.DropColumn(
                name: "codigoPieza",
                table: "Piezas");
        }
    }
}
