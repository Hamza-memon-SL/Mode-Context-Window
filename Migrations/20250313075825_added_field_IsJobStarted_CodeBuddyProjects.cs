using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_field_IsJobStarted_CodeBuddyProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsJobStarted",
                table: "CodeBuddyProjects",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 13, 7, 58, 24, 469, DateTimeKind.Utc).AddTicks(5491), new DateTime(2025, 3, 13, 7, 58, 24, 469, DateTimeKind.Utc).AddTicks(5494) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsJobStarted",
                table: "CodeBuddyProjects");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 9, 22, 46, 199, DateTimeKind.Utc).AddTicks(2737), new DateTime(2025, 3, 10, 9, 22, 46, 199, DateTimeKind.Utc).AddTicks(2740) });
        }
    }
}
