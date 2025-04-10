using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_field_jiraanalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserStoriesId",
                table: "JiraAnalytics",
                newName: "ArtifactsId");

            migrationBuilder.AddColumn<int>(
                name: "ArtifactSearchCount",
                table: "JiraComponentAnalytics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentArtifactId",
                table: "JiraComponentAnalytics",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtifactSearchCount",
                table: "JiraComponentAnalytics");

            migrationBuilder.DropColumn(
                name: "ParentArtifactId",
                table: "JiraComponentAnalytics");

            migrationBuilder.RenameColumn(
                name: "ArtifactsId",
                table: "JiraAnalytics",
                newName: "UserStoriesId");
        }
    }
}
