using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_configurations_fields_backlogBuddy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptanceCriteriaTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CRUDAnalysisTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConformantTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FunctionalSizeTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestScenarioTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 17, 13, 11, 30, 730, DateTimeKind.Utc).AddTicks(6216), new DateTime(2025, 3, 17, 13, 11, 30, 730, DateTimeKind.Utc).AddTicks(6220) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceCriteriaTemplate",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "CRUDAnalysisTemplate",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "ConformantTemplate",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "FunctionalSizeTemplate",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "TestScenarioTemplate",
                table: "Configurations");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 17, 6, 35, 15, 289, DateTimeKind.Utc).AddTicks(1875), new DateTime(2025, 3, 17, 6, 35, 15, 289, DateTimeKind.Utc).AddTicks(1879) });
        }
    }
}
