using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_field_time_jirabaseanalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "JiraBaseAnalytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SearchedDate",
                table: "JiraBaseAnalytics",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "JiraBaseAnalytics");

            migrationBuilder.DropColumn(
                name: "SearchedDate",
                table: "JiraBaseAnalytics");
        }
    }
}
