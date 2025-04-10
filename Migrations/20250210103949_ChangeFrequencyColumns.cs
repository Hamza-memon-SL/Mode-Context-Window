using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFrequencyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "UserTemplateFrequencies",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 10, 10, 39, 48, 483, DateTimeKind.Utc).AddTicks(8256), new DateTime(2025, 2, 10, 10, 39, 48, 483, DateTimeKind.Utc).AddTicks(8261) });

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplateFrequencies_TemplateId",
                table: "UserTemplateFrequencies",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTemplateFrequencies_Templates_TemplateId",
                table: "UserTemplateFrequencies",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTemplateFrequencies_Templates_TemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropIndex(
                name: "IX_UserTemplateFrequencies_TemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 10, 10, 23, 52, 411, DateTimeKind.Utc).AddTicks(6061), new DateTime(2025, 2, 10, 10, 23, 52, 411, DateTimeKind.Utc).AddTicks(6067) });
        }
    }
}
