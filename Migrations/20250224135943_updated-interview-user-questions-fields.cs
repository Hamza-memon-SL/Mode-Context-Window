using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class updatedinterviewuserquestionsfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "UserAuthentication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "InterviewGroupUserQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 24, 13, 59, 42, 770, DateTimeKind.Utc).AddTicks(250), new DateTime(2025, 2, 24, 13, 59, 42, 770, DateTimeKind.Utc).AddTicks(258) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URL",
                table: "UserAuthentication");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "InterviewGroupUserQuestions");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 18, 8, 30, 4, 800, DateTimeKind.Utc).AddTicks(1781), new DateTime(2025, 2, 18, 8, 30, 4, 800, DateTimeKind.Utc).AddTicks(1783) });
        }
    }
}
