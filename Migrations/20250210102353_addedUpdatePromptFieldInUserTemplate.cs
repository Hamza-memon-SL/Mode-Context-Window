using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class addedUpdatePromptFieldInUserTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedTemplatePrompts",
                table: "UserTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 10, 10, 23, 52, 411, DateTimeKind.Utc).AddTicks(6061), new DateTime(2025, 2, 10, 10, 23, 52, 411, DateTimeKind.Utc).AddTicks(6067) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedTemplatePrompts",
                table: "UserTemplates");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 7, 12, 36, 40, 834, DateTimeKind.Utc).AddTicks(8964), new DateTime(2025, 2, 7, 12, 36, 40, 834, DateTimeKind.Utc).AddTicks(8968) });
        }
    }
}
