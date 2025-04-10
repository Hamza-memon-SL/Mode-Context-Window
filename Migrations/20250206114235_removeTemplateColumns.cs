using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class removeTemplateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "TemplateSections");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 6, 11, 42, 34, 683, DateTimeKind.Utc).AddTicks(8047), new DateTime(2025, 2, 6, 11, 42, 34, 683, DateTimeKind.Utc).AddTicks(8051) });

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplates_TemplateSubSectionId",
                table: "UserTemplates",
                column: "TemplateSubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_TemplateCategoryId",
                table: "Templates",
                column: "TemplateCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_TemplateCategories_TemplateCategoryId",
                table: "Templates",
                column: "TemplateCategoryId",
                principalTable: "TemplateCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTemplates_TemplateSubSections_TemplateSubSectionId",
                table: "UserTemplates",
                column: "TemplateSubSectionId",
                principalTable: "TemplateSubSections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_TemplateCategories_TemplateCategoryId",
                table: "Templates");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTemplates_TemplateSubSections_TemplateSubSectionId",
                table: "UserTemplates");

            migrationBuilder.DropIndex(
                name: "IX_UserTemplates_TemplateSubSectionId",
                table: "UserTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_TemplateCategoryId",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Templates");

            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "TemplateSections",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 4, 14, 0, 56, 62, DateTimeKind.Utc).AddTicks(2334), new DateTime(2025, 2, 4, 14, 0, 56, 62, DateTimeKind.Utc).AddTicks(2338) });
        }
    }
}
