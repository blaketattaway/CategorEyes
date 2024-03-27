using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneCore.CategorEyes.Infrastructure.Migrations
{
    public partial class changingdescriptionandtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Historical",
                newName: "Description");

            migrationBuilder.AlterColumn<byte>(
                name: "HistoricalType",
                table: "Historical",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Historical",
                newName: "Data");

            migrationBuilder.AlterColumn<short>(
                name: "HistoricalType",
                table: "Historical",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
