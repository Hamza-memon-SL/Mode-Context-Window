using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class new_table_created_for_main_frame_module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainFrameSourceProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImportURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectChecksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: true),
                    SourceType = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameSourceProject", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainFrameDestinationProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainFrameSourceProjectId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameDestinationProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainFrameDestinationProject_MainFrameSourceProject_MainFrameSourceProjectId",
                        column: x => x.MainFrameSourceProjectId,
                        principalTable: "MainFrameSourceProject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MainFrameSourceFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainFrameSourceProjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileCheckSum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameSourceFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainFrameSourceFiles_MainFrameSourceProject_MainFrameSourceProjectId",
                        column: x => x.MainFrameSourceProjectId,
                        principalTable: "MainFrameSourceProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainFrameDestinationProjectSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainFrameDestinationProjectId = table.Column<int>(type: "int", nullable: true),
                    ExportMethod = table.Column<int>(type: "int", nullable: false),
                    GitHubURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: true),
                    PAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameDestinationProjectSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainFrameDestinationProjectSettings_MainFrameDestinationProject_MainFrameDestinationProjectId",
                        column: x => x.MainFrameDestinationProjectId,
                        principalTable: "MainFrameDestinationProject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MainFrameDestinationFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainFrameDestinationProjectId = table.Column<int>(type: "int", nullable: false),
                    MainFrameSourceFileId = table.Column<int>(type: "int", nullable: false),
                    ChunkFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndexDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StartLine = table.Column<int>(type: "int", nullable: false),
                    EndLine = table.Column<int>(type: "int", nullable: false),
                    MainFrameSourceFilesId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainFrameDestinationFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainFrameDestinationFiles_MainFrameDestinationProject_MainFrameDestinationProjectId",
                        column: x => x.MainFrameDestinationProjectId,
                        principalTable: "MainFrameDestinationProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainFrameDestinationFiles_MainFrameSourceFiles_MainFrameSourceFilesId",
                        column: x => x.MainFrameSourceFilesId,
                        principalTable: "MainFrameSourceFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3078), new DateTime(2025, 2, 24, 12, 6, 35, 199, DateTimeKind.Utc).AddTicks(3080) });

            migrationBuilder.CreateIndex(
                name: "IX_MainFrameDestinationFiles_MainFrameDestinationProjectId",
                table: "MainFrameDestinationFiles",
                column: "MainFrameDestinationProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MainFrameDestinationFiles_MainFrameSourceFilesId",
                table: "MainFrameDestinationFiles",
                column: "MainFrameSourceFilesId");

            migrationBuilder.CreateIndex(
                name: "IX_MainFrameDestinationProject_MainFrameSourceProjectId",
                table: "MainFrameDestinationProject",
                column: "MainFrameSourceProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MainFrameDestinationProjectSettings_MainFrameDestinationProjectId",
                table: "MainFrameDestinationProjectSettings",
                column: "MainFrameDestinationProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MainFrameSourceFiles_MainFrameSourceProjectId",
                table: "MainFrameSourceFiles",
                column: "MainFrameSourceProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainFrameDestinationFiles");

            migrationBuilder.DropTable(
                name: "MainFrameDestinationProjectSettings");

            migrationBuilder.DropTable(
                name: "MainFrameSourceFiles");

            migrationBuilder.DropTable(
                name: "MainFrameDestinationProject");

            migrationBuilder.DropTable(
                name: "MainFrameSourceProject");

            migrationBuilder.UpdateData(
                table: "CustomizationTimeSaved",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 2, 18, 8, 30, 4, 800, DateTimeKind.Utc).AddTicks(1781), new DateTime(2025, 2, 18, 8, 30, 4, 800, DateTimeKind.Utc).AddTicks(1783) });
        }
    }
}
