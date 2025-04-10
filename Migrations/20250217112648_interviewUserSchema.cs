using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class interviewUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interviewGroupResumes_interviewGroups_GroupId",
                table: "interviewGroupResumes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_interviewGroups",
                table: "interviewGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_interviewGroupResumes",
                table: "interviewGroupResumes");

            migrationBuilder.RenameTable(
                name: "interviewGroups",
                newName: "InterviewGroups");

            migrationBuilder.RenameTable(
                name: "interviewGroupResumes",
                newName: "InterviewGroupResumes");

            migrationBuilder.RenameIndex(
                name: "IX_interviewGroupResumes_GroupId",
                table: "InterviewGroupResumes",
                newName: "IX_InterviewGroupResumes_GroupId");

            migrationBuilder.AddColumn<bool>(
                name: "IsLlmInteracted",
                table: "InterviewGroupResumes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LlmInteractionDate",
                table: "InterviewGroupResumes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterviewGroups",
                table: "InterviewGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterviewGroupResumes",
                table: "InterviewGroupResumes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InterviewGroupUserDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewGroupId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewGroupUserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewGroupUserDetails_InterviewGroups_InterviewGroupId",
                        column: x => x.InterviewGroupId,
                        principalTable: "InterviewGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InterviewGroupUserQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewGroupUserDetailId = table.Column<int>(type: "int", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImpactFactor = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewGroupUserQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewGroupUserQuestions_InterviewGroupUserDetails_InterviewGroupUserDetailId",
                        column: x => x.InterviewGroupUserDetailId,
                        principalTable: "InterviewGroupUserDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 17, 11, 26, 47, 404, DateTimeKind.Utc).AddTicks(7734), new DateTime(2025, 2, 17, 11, 26, 47, 404, DateTimeKind.Utc).AddTicks(7737) });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewGroupUserDetails_InterviewGroupId",
                table: "InterviewGroupUserDetails",
                column: "InterviewGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewGroupUserQuestions_InterviewGroupUserDetailId",
                table: "InterviewGroupUserQuestions",
                column: "InterviewGroupUserDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewGroupResumes_InterviewGroups_GroupId",
                table: "InterviewGroupResumes",
                column: "GroupId",
                principalTable: "InterviewGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewGroupResumes_InterviewGroups_GroupId",
                table: "InterviewGroupResumes");

            migrationBuilder.DropTable(
                name: "InterviewGroupUserQuestions");

            migrationBuilder.DropTable(
                name: "InterviewGroupUserDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterviewGroups",
                table: "InterviewGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterviewGroupResumes",
                table: "InterviewGroupResumes");

            migrationBuilder.DropColumn(
                name: "IsLlmInteracted",
                table: "InterviewGroupResumes");

            migrationBuilder.DropColumn(
                name: "LlmInteractionDate",
                table: "InterviewGroupResumes");

            migrationBuilder.RenameTable(
                name: "InterviewGroups",
                newName: "interviewGroups");

            migrationBuilder.RenameTable(
                name: "InterviewGroupResumes",
                newName: "interviewGroupResumes");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewGroupResumes_GroupId",
                table: "interviewGroupResumes",
                newName: "IX_interviewGroupResumes_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_interviewGroups",
                table: "interviewGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_interviewGroupResumes",
                table: "interviewGroupResumes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 14, 13, 37, 25, 855, DateTimeKind.Utc).AddTicks(4348), new DateTime(2025, 2, 14, 13, 37, 25, 855, DateTimeKind.Utc).AddTicks(4351) });

            migrationBuilder.AddForeignKey(
                name: "FK_interviewGroupResumes_interviewGroups_GroupId",
                table: "interviewGroupResumes",
                column: "GroupId",
                principalTable: "interviewGroups",
                principalColumn: "Id");
        }
    }
}
