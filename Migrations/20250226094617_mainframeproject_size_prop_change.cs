using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class mainframeproject_size_prop_change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "MainFrameSourceFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 26, 9, 46, 16, 799, DateTimeKind.Utc).AddTicks(7393), new DateTime(2025, 2, 26, 9, 46, 16, 799, DateTimeKind.Utc).AddTicks(7397) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "MainFrameSourceFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3078), new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3080) });
        }
    }
}
