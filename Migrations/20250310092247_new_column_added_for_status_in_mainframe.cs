using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_column_added_for_status_in_mainframe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MainFrameDestinationProject",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MainFrameDestinationFiles",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 9, 22, 46, 199, DateTimeKind.Utc).AddTicks(2737), new DateTime(2025, 3, 10, 9, 22, 46, 199, DateTimeKind.Utc).AddTicks(2740) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MainFrameDestinationProject");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MainFrameDestinationFiles");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 58, 26, 716, DateTimeKind.Utc).AddTicks(8867), new DateTime(2025, 3, 10, 6, 58, 26, 716, DateTimeKind.Utc).AddTicks(8876) });
        }
    }
}
