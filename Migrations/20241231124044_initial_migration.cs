using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModel",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    InputTokenPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutputTokenPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModel", x => x.ModelId);
                });

            migrationBuilder.CreateTable(
                name: "BacklogBuddyProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUserStoryHealth = table.Column<bool>(type: "bit", nullable: true),
                    IsUseCaseModel = table.Column<bool>(type: "bit", nullable: true),
                    IsCrudAnalysis = table.Column<bool>(type: "bit", nullable: true),
                    IsFunctionalSize = table.Column<bool>(type: "bit", nullable: true),
                    IsTestScenarios = table.Column<bool>(type: "bit", nullable: true),
                    IsEpicProgressOverview = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogBuddyProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoilerPlatesTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoilerPlatesTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatHistoryId = table.Column<int>(type: "int", nullable: false),
                    FeedbackType = table.Column<int>(type: "int", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromptId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatType = table.Column<int>(type: "int", nullable: true),
                    HistoryChatType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromptText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPrompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeBuddyProjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyProjectGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeBuddyProjectInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InviteToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyProjectInvites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeBuddyProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GitHubURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndexDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualInputTokens = table.Column<int>(type: "int", nullable: true),
                    ActualOutputTokens = table.Column<int>(type: "int", nullable: true),
                    VisualRepresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectChecksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInMemory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfLinesCoppied = table.Column<int>(type: "int", nullable: false),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HistoryChatType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSuggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingleFileSummaryPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SingleFileJsonPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectFileSummaryPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectFileJsonPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SingleFileSummaryTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectFileSummaryTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SingleFileJsonTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectFileJsonTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimpleUserStoryTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplexUserStoryTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JsonValidationRetryAttempt = table.Column<int>(type: "int", nullable: false),
                    NonConformancePrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmbiguousPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SemanticPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevopsUserStories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserStoriesId = table.Column<int>(type: "int", nullable: true),
                    DevopsProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamProject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IterationPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStoriesCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoardColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    NonConformanceScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmbigiousScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemanticScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonConformanceSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmbigiousScoreSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemanticSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LlmInteractionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LlmInteractionModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevopsUserStories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JiraUserStories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserStoriesId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoryType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoryPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStoriesStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStoriesCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserStoriesUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NonConformanceScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmbigiousScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemanticScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonConformanceSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmbigiousScoreSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemanticSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LlmInteractionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LlmInteractionModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraUserStories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAuthentication",
                columns: table => new
                {
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevopsBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DevopsPAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JiraPAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "CodeBuddyFileDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssumedInputTokens = table.Column<int>(type: "int", nullable: true),
                    ActualInputTokens = table.Column<int>(type: "int", nullable: true),
                    ActualOutputTokens = table.Column<int>(type: "int", nullable: true),
                    VisualRepresentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileChecksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndexDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyFileDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeBuddyFileDetails_CodeBuddyProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CodeBuddyProjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CodeBuddyProjectSettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    CloudPlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtensionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBuddyProjectSettings", x => x.SettingId);
                    table.ForeignKey(
                        name: "FK_CodeBuddyProjectSettings_CodeBuddyProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CodeBuddyProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChatPrompts",
                columns: new[] { "Id", "IsActive", "OrderId", "PromptText", "Question" },
                values: new object[,]
                {
                    { 1, true, 1, "Please do this needful action", "Combine these files as one" },
                    { 2, true, 2, "Please do this needful action", "Convert into Java" },
                    { 3, true, 3, "Please do this needful action", "Convert into Python" },
                    { 4, true, 4, "Please do this needful action", "Generate Documentation" },
                    { 5, true, 5, "Please do this needful action", "Translate this code into TypeScript" },
                    { 6, true, 6, "Please do this needful action", "Optimize the performance of this code" },
                    { 7, true, 7, "Please do this needful action", "Refactor the code for readability" },
                    { 8, true, 8, "Please do this needful action", "Find bugs in this program" },
                    { 9, true, 9, "Please do this needful action", "Write unit tests for the following code" },
                    { 10, true, 10, "Please do this needful action", "Generate a summary for this dataset" },
                    { 11, true, 11, "Please do this needful action", "Compare this algorithm with a better approach" },
                    { 12, true, 12, "Please do this needful action", "Improve the error handling in this code" },
                    { 13, true, 13, "Please do this needful action", "Break down the logic in this code block" },
                    { 14, true, 14, "Please do this needful action", "Update the syntax for the latest version" },
                    { 15, true, 15, "Please do this needful action", "Generate a UML diagram from this code" },
                    { 16, true, 16, "Please do this needful action", "What’s the time complexity of this function?" },
                    { 17, true, 17, "Please do this needful action", "Make this code more secure" },
                    { 18, true, 18, "Please do this needful action", "Add comments to explain this code" },
                    { 19, true, 19, "Please do this needful action", "Can you simplify this logic?" },
                    { 20, true, 20, "Please do this needful action", "Generate a summary for this dataset" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeBuddyFileDetails_ProjectId",
                table: "CodeBuddyFileDetails",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeBuddyProjectSettings_ProjectId",
                table: "CodeBuddyProjectSettings",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIModel");

            migrationBuilder.DropTable(
                name: "BacklogBuddyProjects");

            migrationBuilder.DropTable(
                name: "BoilerPlatesTemplates");

            migrationBuilder.DropTable(
                name: "ChatFeedbacks");

            migrationBuilder.DropTable(
                name: "ChatHistory");

            migrationBuilder.DropTable(
                name: "ChatPrompts");

            migrationBuilder.DropTable(
                name: "CodeBuddyFileDetails");

            migrationBuilder.DropTable(
                name: "CodeBuddyProjectGroups");

            migrationBuilder.DropTable(
                name: "CodeBuddyProjectInvites");

            migrationBuilder.DropTable(
                name: "CodeBuddyProjectSettings");

            migrationBuilder.DropTable(
                name: "CodeSuggestions");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "DevopsUserStories");

            migrationBuilder.DropTable(
                name: "JiraUserStories");

            migrationBuilder.DropTable(
                name: "UserAuthentication");

            migrationBuilder.DropTable(
                name: "CodeBuddyProjects");
        }
    }
}
