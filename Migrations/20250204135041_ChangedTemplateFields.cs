using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTemplateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateSectionId",
                table: "UserTemplates",
                newName: "SubSectionOrder");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 4, 13, 50, 40, 404, DateTimeKind.Utc).AddTicks(3546), new DateTime(2025, 2, 4, 13, 50, 40, 404, DateTimeKind.Utc).AddTicks(3551) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubSectionOrder",
                table: "UserTemplates",
                newName: "TemplateSectionId");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 4, 12, 34, 50, 996, DateTimeKind.Utc).AddTicks(3334), new DateTime(2025, 2, 4, 12, 34, 50, 996, DateTimeKind.Utc).AddTicks(3340) });
        }
    }
}
