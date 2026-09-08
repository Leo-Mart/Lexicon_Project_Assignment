using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReworkSubmissionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");

            migrationBuilder.AddColumn<int>(
                name: "ReviewStatus",
                table: "Submissions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "Submissions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
