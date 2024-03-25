using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneCore.CategorEyes.Infrastructure.Migrations
{
    public partial class historicaltype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Historical",
                newName: "HistoricalType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HistoricalType",
                table: "Historical",
                newName: "MyProperty");
        }
    }
}
