using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedPromptTitleFieldInUserTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedTemplateTitle",
                table: "UserTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 10, 11, 52, 8, 320, DateTimeKind.Utc).AddTicks(9880), new DateTime(2025, 2, 10, 11, 52, 8, 320, DateTimeKind.Utc).AddTicks(9884) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedTemplateTitle",
                table: "UserTemplates");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 10, 10, 39, 48, 483, DateTimeKind.Utc).AddTicks(8256), new DateTime(2025, 2, 10, 10, 39, 48, 483, DateTimeKind.Utc).AddTicks(8261) });
        }
    }
}
