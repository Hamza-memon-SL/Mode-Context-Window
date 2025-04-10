using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_status_fields_codebuddy : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "MainFrameSourceFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "CodeBuddyProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClonedFailed",
                table: "CodeBuddyProjects",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProjectClonedByJob",
                table: "CodeBuddyProjects",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 6, 7, 55, 16, 958, DateTimeKind.Utc).AddTicks(5893), new DateTime(2025, 3, 6, 7, 55, 16, 958, DateTimeKind.Utc).AddTicks(5897) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "CodeBuddyProjects");

            migrationBuilder.DropColumn(
                name: "IsClonedFailed",
                table: "CodeBuddyProjects");

            migrationBuilder.DropColumn(
                name: "IsProjectClonedByJob",
                table: "CodeBuddyProjects");

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
