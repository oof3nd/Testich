using Microsoft.EntityFrameworkCore.Migrations;

namespace Testich.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "LevelsOfTest",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "LevelsOfTest",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
