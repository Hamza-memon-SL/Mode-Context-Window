using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class changed_entityname_jiraanalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JiraComponentAnalytics");

            migrationBuilder.DropTable(
                name: "JiraAnalytics");

            migrationBuilder.CreateTable(
                name: "JiraBaseAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    newFetchIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updatedFetchIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraBaseAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JiraSearchAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtifactsId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStoriesStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FrequencySearchCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraSearchAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JiraSearchArtifactsAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jiraAnalyticsId = table.Column<int>(type: "int", nullable: false),
                    ParentArtifactId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArtifactSearchCount = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraSearchArtifactsAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JiraSearchArtifactsAnalytics_JiraSearchAnalytics_jiraAnalyticsId",
                        column: x => x.jiraAnalyticsId,
                        principalTable: "JiraSearchAnalytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JiraSearchArtifactsAnalytics_jiraAnalyticsId",
                table: "JiraSearchArtifactsAnalytics",
                column: "jiraAnalyticsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JiraBaseAnalytics");

            migrationBuilder.DropTable(
                name: "JiraSearchArtifactsAnalytics");

            migrationBuilder.DropTable(
                name: "JiraSearchAnalytics");

            migrationBuilder.CreateTable(
                name: "JiraAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtifactsId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FrequencySearchCount = table.Column<int>(type: "int", nullable: false),
                    JiraProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStoriesStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JiraComponentAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jiraAnalyticsId = table.Column<int>(type: "int", nullable: false),
                    ArtifactSearchCount = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentArtifactId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraComponentAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JiraComponentAnalytics_JiraAnalytics_jiraAnalyticsId",
                        column: x => x.jiraAnalyticsId,
                        principalTable: "JiraAnalytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JiraComponentAnalytics_jiraAnalyticsId",
                table: "JiraComponentAnalytics",
                column: "jiraAnalyticsId");
        }
    }
}
