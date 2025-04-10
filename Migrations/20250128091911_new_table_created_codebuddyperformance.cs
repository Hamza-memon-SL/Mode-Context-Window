using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_table_created_codebuddyperformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeBuddyPerformance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestTimeT1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseTimeT2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DifferenceTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyPerformance", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 28, 9, 19, 11, 347, DateTimeKind.Utc).AddTicks(9841), new DateTime(2025, 1, 28, 9, 19, 11, 347, DateTimeKind.Utc).AddTicks(9846) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeBuddyPerformance");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 28, 9, 17, 38, 642, DateTimeKind.Utc).AddTicks(360), new DateTime(2025, 1, 28, 9, 17, 38, 642, DateTimeKind.Utc).AddTicks(364) });
        }
    }
}
