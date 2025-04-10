using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_fields_in_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptanceCriteria",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptanceCriteriaJson",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConformanceSuggestion",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrudAnalysisJson",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FunctionalSizeJson",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryCompletenessJson",
                table: "DevopsUserStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryCompletnessTemplate",
                table: "Configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 19, 8, 0, 27, 863, DateTimeKind.Utc).AddTicks(8257), new DateTime(2025, 3, 19, 8, 0, 27, 863, DateTimeKind.Utc).AddTicks(8260) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceCriteria",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "AcceptanceCriteriaJson",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "ConformanceSuggestion",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "CrudAnalysisJson",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "FunctionalSizeJson",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "StoryCompletenessJson",
                table: "DevopsUserStories");

            migrationBuilder.DropColumn(
                name: "StoryCompletnessTemplate",
                table: "Configurations");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 17, 13, 11, 30, 730, DateTimeKind.Utc).AddTicks(6216), new DateTime(2025, 3, 17, 13, 11, 30, 730, DateTimeKind.Utc).AddTicks(6220) });
        }
    }
}
