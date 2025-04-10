using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class interviewDirtyFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "InterviewGroupResumes",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 17, 13, 56, 19, 319, DateTimeKind.Utc).AddTicks(3315), new DateTime(2025, 2, 17, 13, 56, 19, 319, DateTimeKind.Utc).AddTicks(3318) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "InterviewGroupResumes");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 17, 11, 26, 47, 404, DateTimeKind.Utc).AddTicks(7734), new DateTime(2025, 2, 17, 11, 26, 47, 404, DateTimeKind.Utc).AddTicks(7737) });
        }
    }
}
