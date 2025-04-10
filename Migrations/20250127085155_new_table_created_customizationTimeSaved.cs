using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_table_created_customizationTimeSaved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomizationTimeSaved",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimatedTimeSavedLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedTimeSavedFormulaValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedTimeSavedFormula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedTimeSavedDashboardFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedTimeSavedVisibotFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationTimeSaved", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CustomizationTimeSaved",
                columns: new[] { "Id", "CreatedDate", "EstimatedTimeSavedDashboardFormat", "EstimatedTimeSavedFormula", "EstimatedTimeSavedFormulaValue", "EstimatedTimeSavedLabel", "EstimatedTimeSavedVisibotFormat", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 27, 8, 51, 54, 553, DateTimeKind.Utc).AddTicks(6428), "{d} days, {h} hours and {m} minutes", "(Total Characters Copied/ Typing Speed) + (Total Characters Copied * Logic Time) + (Total Characters Copied * Testing Time)", "({0}/40) + ({0} * 0.05) + ({0} * 0.04)", "(Total Characters Copied/40) + (Total Characters Copied * 0.05) + (Total Characters Copied * 0.04)", "{hh.mm} Hrs", true, new DateTime(2025, 1, 27, 8, 51, 54, 553, DateTimeKind.Utc).AddTicks(6432) }
                 
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomizationTimeSaved");
        }
    }
}
