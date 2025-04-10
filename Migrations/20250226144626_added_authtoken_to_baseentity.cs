using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_authtoken_to_baseentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "UserTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "UserTemplateFrequencies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "TemplateSubSections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "TemplateSections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "TemplateCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "ModelVersionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "MainFrameSourceProject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "MainFrameSourceFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "MainFrameDestinationProjectSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "MainFrameDestinationProject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "MainFrameDestinationFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "InterviewGroupUserQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "InterviewGroupUserDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "InterviewGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "InterviewGroupResumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "ExtensionBuilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "DownloadModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "AboutModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 26, 14, 46, 25, 496, DateTimeKind.Utc).AddTicks(3693), new DateTime(2025, 2, 26, 14, 46, 25, 496, DateTimeKind.Utc).AddTicks(3696) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "UserTemplates");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "UserTemplateFrequencies");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "TemplateSubSections");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "TemplateSections");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "TemplateCategories");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "ModelVersionDetails");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "MainFrameSourceProject");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "MainFrameSourceFiles");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "MainFrameDestinationProjectSettings");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "MainFrameDestinationProject");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "MainFrameDestinationFiles");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "InterviewGroupUserQuestions");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "InterviewGroupUserDetails");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "InterviewGroups");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "InterviewGroupResumes");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "ExtensionBuilds");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "DownloadModels");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "AboutModels");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3078), new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3080) });
        }
    }
}
