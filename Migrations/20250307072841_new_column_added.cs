using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_column_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTemplateFrequencies_UserTemplates_UserTemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropIndex(
                name: "IX_UserTemplateFrequencies_UserTemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropColumn(
                name: "UserTemplateId",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropColumn(
                name: "MainFrameSourceFileId",
                table: "MainFrameDestinationFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "MainFrameSourceFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "MainFrameDestinationFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "MainFrameDestinationFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 7, 7, 28, 40, 579, DateTimeKind.Utc).AddTicks(1514), new DateTime(2025, 3, 7, 7, 28, 40, 579, DateTimeKind.Utc).AddTicks(1519) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "MainFrameDestinationFiles");


            migrationBuilder.AddColumn<int>(
                name: "UserTemplateId",
                table: "UserTemplateFrequencies",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "MainFrameSourceFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "MainFrameDestinationFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MainFrameSourceFileId",
                table: "MainFrameDestinationFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 26, 14, 46, 25, 496, DateTimeKind.Utc).AddTicks(3693), new DateTime(2025, 2, 26, 14, 46, 25, 496, DateTimeKind.Utc).AddTicks(3696) });

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplateFrequencies_UserTemplateId",
                table: "UserTemplateFrequencies",
                column: "UserTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTemplateFrequencies_UserTemplates_UserTemplateId",
                table: "UserTemplateFrequencies",
                column: "UserTemplateId",
                principalTable: "UserTemplates",
                principalColumn: "Id");
        }
    }
}
