using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_colum_added_in_codebuddyperformance_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HistoryChatType",
                table: "CodeBuddyPerformance",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 28, 10, 38, 30, 64, DateTimeKind.Utc).AddTicks(3284), new DateTime(2025, 1, 28, 10, 38, 30, 64, DateTimeKind.Utc).AddTicks(3290) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoryChatType",
                table: "CodeBuddyPerformance");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 28, 9, 19, 11, 347, DateTimeKind.Utc).AddTicks(9841), new DateTime(2025, 1, 28, 9, 19, 11, 347, DateTimeKind.Utc).AddTicks(9846) });
        }
    }
}
