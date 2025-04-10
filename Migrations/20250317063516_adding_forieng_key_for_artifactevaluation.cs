using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class adding_forieng_key_for_artifactevaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 17, 6, 35, 15, 289, DateTimeKind.Utc).AddTicks(1875), new DateTime(2025, 3, 17, 6, 35, 15, 289, DateTimeKind.Utc).AddTicks(1879) });

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactEvaluationCriterias_ArtifactCategoryId",
                table: "ArtifactEvaluationCriterias",
                column: "ArtifactCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtifactEvaluationCriterias_ArtifactCategories_ArtifactCategoryId",
                table: "ArtifactEvaluationCriterias",
                column: "ArtifactCategoryId",
                principalTable: "ArtifactCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtifactEvaluationCriterias_ArtifactCategories_ArtifactCategoryId",
                table: "ArtifactEvaluationCriterias");

            migrationBuilder.DropIndex(
                name: "IX_ArtifactEvaluationCriterias_ArtifactCategoryId",
                table: "ArtifactEvaluationCriterias");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 3, 17, 5, 46, 35, 591, DateTimeKind.Utc).AddTicks(9716), new DateTime(2025, 3, 17, 5, 46, 35, 591, DateTimeKind.Utc).AddTicks(9725) });
        }
    }
}
