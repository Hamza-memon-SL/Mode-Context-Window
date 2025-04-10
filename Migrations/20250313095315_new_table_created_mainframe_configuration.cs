using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_table_created_mainframe_configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "MainFrameConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimpleSummarizeCodePrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SingleFileSummarizePrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeSplitSuggestionPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeSplitSuggestionJsonTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameConfigurations", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 13, 9, 53, 14, 516, DateTimeKind.Utc).AddTicks(202), new DateTime(2025, 3, 13, 9, 53, 14, 516, DateTimeKind.Utc).AddTicks(205) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainFrameConfigurations");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 13, 7, 58, 24, 469, DateTimeKind.Utc).AddTicks(5491), new DateTime(2025, 3, 13, 7, 58, 24, 469, DateTimeKind.Utc).AddTicks(5494) });
        }
    }
}
