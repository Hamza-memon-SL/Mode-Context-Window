using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_fields_codebuddy_files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndexedType",
                table: "CodeBuddyProjectSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IndexedBy",
                table: "CodeBuddyFileDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLastIndexed",
                table: "CodeBuddyFileDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 10, 51, 29, 912, DateTimeKind.Utc).AddTicks(7062), new DateTime(2025, 3, 10, 10, 51, 29, 912, DateTimeKind.Utc).AddTicks(7067) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexedType",
                table: "CodeBuddyProjectSettings");

            migrationBuilder.DropColumn(
                name: "IndexedBy",
                table: "CodeBuddyFileDetails");

            migrationBuilder.DropColumn(
                name: "IsLastIndexed",
                table: "CodeBuddyFileDetails");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 58, 26, 716, DateTimeKind.Utc).AddTicks(8867), new DateTime(2025, 3, 10, 6, 58, 26, 716, DateTimeKind.Utc).AddTicks(8876) });
        }
    }
}
