using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class addTemplateIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 7, 12, 36, 40, 834, DateTimeKind.Utc).AddTicks(8964), new DateTime(2025, 2, 7, 12, 36, 40, 834, DateTimeKind.Utc).AddTicks(8968) });

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplateFrequencies_UserTemplateId",
                table: "UserTemplateFrequencies",
                column: "UserTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateSubSections_TemplateSectionId",
                table: "TemplateSubSections",
                column: "TemplateSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateSubSections_TemplateSections_TemplateSectionId",
                table: "TemplateSubSections",
                column: "TemplateSectionId",
                principalTable: "TemplateSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTemplateFrequencies_UserTemplates_UserTemplateId",
                table: "UserTemplateFrequencies",
                column: "UserTemplateId",
                principalTable: "UserTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateSubSections_TemplateSections_TemplateSectionId",
                table: "TemplateSubSections");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTemplateFrequencies_UserTemplates_UserTemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropIndex(
                name: "IX_UserTemplateFrequencies_UserTemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropIndex(
                name: "IX_TemplateSubSections_TemplateSectionId",
                table: "TemplateSubSections");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 6, 11, 42, 34, 683, DateTimeKind.Utc).AddTicks(8047), new DateTime(2025, 2, 6, 11, 42, 34, 683, DateTimeKind.Utc).AddTicks(8051) });
        }
    }
}
