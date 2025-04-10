using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class remove_index_project_settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 12, 30, 45, 580, DateTimeKind.Utc).AddTicks(1769), new DateTime(2025, 3, 10, 12, 30, 45, 580, DateTimeKind.Utc).AddTicks(1773) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 10, 51, 29, 912, DateTimeKind.Utc).AddTicks(7062), new DateTime(2025, 3, 10, 10, 51, 29, 912, DateTimeKind.Utc).AddTicks(7067) });
        }
    }
}
