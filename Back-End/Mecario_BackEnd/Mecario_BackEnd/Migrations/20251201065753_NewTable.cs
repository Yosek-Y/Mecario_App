using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mecario_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class NewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiciosMecanicos",
                columns: table => new
                {
                    idServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    servicio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipoServicio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosMecanicos", x => x.idServicio);
                });

            migrationBuilder.CreateTable(
                name: "OrdenServicio_Servicio",
                columns: table => new
                {
                    idOrden = table.Column<int>(type: "int", nullable: false),
                    idServicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicio_Servicio", x => new { x.idOrden, x.idServicio });
                    table.ForeignKey(
                        name: "FK_OrdenServicio_Servicio_OrdenesServicios_idOrden",
                        column: x => x.idOrden,
                        principalTable: "OrdenesServicios",
                        principalColumn: "idOrden",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenServicio_Servicio_ServiciosMecanicos_idServicio",
                        column: x => x.idServicio,
                        principalTable: "ServiciosMecanicos",
                        principalColumn: "idServicio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicio_Servicio_idServicio",
                table: "OrdenServicio_Servicio",
                column: "idServicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenServicio_Servicio");

            migrationBuilder.DropTable(
                name: "ServiciosMecanicos");
        }
    }
}
