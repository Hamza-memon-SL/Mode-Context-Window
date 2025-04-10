using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class addedtemplateindexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 4, 14, 0, 56, 62, DateTimeKind.Utc).AddTicks(2334), new DateTime(2025, 2, 4, 14, 0, 56, 62, DateTimeKind.Utc).AddTicks(2338) });

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplates_TemplateId",
                table: "UserTemplates",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTemplates_Templates_TemplateId",
                table: "UserTemplates",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTemplates_Templates_TemplateId",
                table: "UserTemplates");

            migrationBuilder.DropIndex(
                name: "IX_UserTemplates_TemplateId",
                table: "UserTemplates");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 4, 13, 50, 40, 404, DateTimeKind.Utc).AddTicks(3546), new DateTime(2025, 2, 4, 13, 50, 40, 404, DateTimeKind.Utc).AddTicks(3551) });
        }
    }
}
