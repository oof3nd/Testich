using Microsoft.EntityFrameworkCore.Migrations;

namespace Testich.Migrations
{
    public partial class InitialCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClosedQuestions_QuestionOfTestId",
                table: "ClosedQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestions_QuestionOfTestId",
                table: "ClosedQuestions",
                column: "QuestionOfTestId",
                unique: true,
                filter: "[QuestionOfTestId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClosedQuestions_QuestionOfTestId",
                table: "ClosedQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestions_QuestionOfTestId",
                table: "ClosedQuestions",
                column: "QuestionOfTestId");
        }
    }
}
