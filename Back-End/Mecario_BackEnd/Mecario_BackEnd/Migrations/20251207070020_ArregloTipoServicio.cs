using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mecario_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ArregloTipoServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tipoServicio",
                table: "ServiciosMecanicos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tipoServicio",
                table: "ServiciosMecanicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
