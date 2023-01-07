using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A.Data.Migrations
{
    public partial class PrecoDiario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PrecoDiario",
                table: "Veiculo",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoDiario",
                table: "Veiculo");
        }
    }
}
