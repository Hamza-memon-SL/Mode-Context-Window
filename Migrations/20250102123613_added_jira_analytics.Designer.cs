﻿// <auto-generated />
using System;
using GenAiPoc.Contracts.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    [DbContext(typeof(DbContextGenAiPOC))]
    [Migration("20250102123613_added_jira_analytics")]
    partial class added_jira_analytics
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GenAiPoc.Contracts.Models.AIModel", b =>
                {
                    b.Property<int>("ModelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ModelId"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("InputTokenPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModelType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("OutputTokenPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("ModelId");

                    b.ToTable("AIModel", (string)null);
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyFileDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ActualInputTokens")
                        .HasColumnType("int");

                    b.Property<int?>("ActualOutputTokens")
                        .HasColumnType("int");

                    b.Property<int?>("AssumedInputTokens")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Extension")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileChecksum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FileType")
                        .HasColumnType("int");

                    b.Property<string>("FullPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("IndexDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("Size")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VisualRepresentation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("CodeBuddyFileDetails", (string)null);
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyProjectSettings", b =>
                {
                    b.Property<int>("SettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SettingId"));

                    b.Property<string>("CloudPlatform")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ExtensionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("SettingId");

                    b.HasIndex("ProjectId")
                        .IsUnique();

                    b.ToTable("CodeBuddyProjectSettings", (string)null);
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyProjects", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ActualInputTokens")
                        .HasColumnType("int");

                    b.Property<int?>("ActualOutputTokens")
                        .HasColumnType("int");

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GitHubURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("IndexDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsInMemory")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectChecksum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VisualRepresentation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CodeBuddyProjects", (string)null);
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.BacklogBuddyProjects", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsCrudAnalysis")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsEpicProgressOverview")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsFunctionalSize")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsTestScenarios")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsUseCaseModel")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsUserStoryHealth")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProjectId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectURL")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BacklogBuddyProjects");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.BoilerPlatesTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileSize")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BoilerPlatesTemplates");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.ChatFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChatHistoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FeedbackType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ChatFeedbacks");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.ChatHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ChatType")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("HistoryChatType")
                        .HasColumnType("int");

                    b.Property<int>("PromptId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ChatHistory");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.ChatPrompts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("PromptText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ChatPrompts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsActive = true,
                            OrderId = 1,
                            PromptText = "Please do this needful action",
                            Question = "Combine these files as one"
                        },
                        new
                        {
                            Id = 2,
                            IsActive = true,
                            OrderId = 2,
                            PromptText = "Please do this needful action",
                            Question = "Convert into Java"
                        },
                        new
                        {
                            Id = 3,
                            IsActive = true,
                            OrderId = 3,
                            PromptText = "Please do this needful action",
                            Question = "Convert into Python"
                        },
                        new
                        {
                            Id = 4,
                            IsActive = true,
                            OrderId = 4,
                            PromptText = "Please do this needful action",
                            Question = "Generate Documentation"
                        },
                        new
                        {
                            Id = 5,
                            IsActive = true,
                            OrderId = 5,
                            PromptText = "Please do this needful action",
                            Question = "Translate this code into TypeScript"
                        },
                        new
                        {
                            Id = 6,
                            IsActive = true,
                            OrderId = 6,
                            PromptText = "Please do this needful action",
                            Question = "Optimize the performance of this code"
                        },
                        new
                        {
                            Id = 7,
                            IsActive = true,
                            OrderId = 7,
                            PromptText = "Please do this needful action",
                            Question = "Refactor the code for readability"
                        },
                        new
                        {
                            Id = 8,
                            IsActive = true,
                            OrderId = 8,
                            PromptText = "Please do this needful action",
                            Question = "Find bugs in this program"
                        },
                        new
                        {
                            Id = 9,
                            IsActive = true,
                            OrderId = 9,
                            PromptText = "Please do this needful action",
                            Question = "Write unit tests for the following code"
                        },
                        new
                        {
                            Id = 10,
                            IsActive = true,
                            OrderId = 10,
                            PromptText = "Please do this needful action",
                            Question = "Generate a summary for this dataset"
                        },
                        new
                        {
                            Id = 11,
                            IsActive = true,
                            OrderId = 11,
                            PromptText = "Please do this needful action",
                            Question = "Compare this algorithm with a better approach"
                        },
                        new
                        {
                            Id = 12,
                            IsActive = true,
                            OrderId = 12,
                            PromptText = "Please do this needful action",
                            Question = "Improve the error handling in this code"
                        },
                        new
                        {
                            Id = 13,
                            IsActive = true,
                            OrderId = 13,
                            PromptText = "Please do this needful action",
                            Question = "Break down the logic in this code block"
                        },
                        new
                        {
                            Id = 14,
                            IsActive = true,
                            OrderId = 14,
                            PromptText = "Please do this needful action",
                            Question = "Update the syntax for the latest version"
                        },
                        new
                        {
                            Id = 15,
                            IsActive = true,
                            OrderId = 15,
                            PromptText = "Please do this needful action",
                            Question = "Generate a UML diagram from this code"
                        },
                        new
                        {
                            Id = 16,
                            IsActive = true,
                            OrderId = 16,
                            PromptText = "Please do this needful action",
                            Question = "What’s the time complexity of this function?"
                        },
                        new
                        {
                            Id = 17,
                            IsActive = true,
                            OrderId = 17,
                            PromptText = "Please do this needful action",
                            Question = "Make this code more secure"
                        },
                        new
                        {
                            Id = 18,
                            IsActive = true,
                            OrderId = 18,
                            PromptText = "Please do this needful action",
                            Question = "Add comments to explain this code"
                        },
                        new
                        {
                            Id = 19,
                            IsActive = true,
                            OrderId = 19,
                            PromptText = "Please do this needful action",
                            Question = "Can you simplify this logic?"
                        },
                        new
                        {
                            Id = 20,
                            IsActive = true,
                            OrderId = 20,
                            PromptText = "Please do this needful action",
                            Question = "Generate a summary for this dataset"
                        });
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.CodeBuddyProjectGroups", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CodeBuddyProjectGroups");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.CodeBuddyProjectInvites", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InviteToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("bit");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CodeBuddyProjectInvites");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.CodeSuggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("HistoryChatType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfLinesCoppied")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("Query")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CodeSuggestions");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.Configurations", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AmbiguousPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ComplexUserStoryTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("JsonValidationRetryAttempt")
                        .HasColumnType("int");

                    b.Property<string>("NonConformancePrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectFileJsonPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectFileJsonTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectFileSummaryPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectFileSummaryTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SemanticPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SimpleUserStoryTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SingleFileJsonPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SingleFileJsonTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SingleFileSummaryPrompt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SingleFileSummaryTemplate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.DevopsUserStories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AmbigiousScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmbigiousScoreSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AreaPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BoardColumn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DevopsProjectId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IterationPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LlmInteractionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LlmInteractionModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NonConformanceScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NonConformanceSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SemanticScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SemanticSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StateChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TeamProject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UserStoriesCreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserStoriesId")
                        .HasColumnType("int");

                    b.Property<string>("WorkItemType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DevopsUserStories");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.JiraAnalytics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FrequencySearchCount")
                        .HasColumnType("int");

                    b.Property<string>("JiraProjectId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("Score")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SearchType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserStoriesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserStoriesStatus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JiraAnalytics");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.JiraComponentAnalytics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ComponentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Relationship")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SearchType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("jiraAnalyticsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("jiraAnalyticsId");

                    b.ToTable("JiraComponentAnalytics");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.JiraUserStories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AmbigiousScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmbigiousScoreSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraProjectId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LlmInteractionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LlmInteractionModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NonConformanceScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NonConformanceSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Priority")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SemanticScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SemanticSuggestion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoryPoint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoryType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UserStoriesCreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserStoriesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserStoriesStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UserStoriesUpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("JiraUserStories");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.UserAuthentication", b =>
                {
                    b.Property<string>("AuthToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DevopsBaseUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DevopsPAT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraBaseUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraPAT")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("UserAuthentication");
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyFileDetails", b =>
                {
                    b.HasOne("GenAiPoc.Contracts.Models.CodeBuddyProjects", "Project")
                        .WithMany("Files")
                        .HasForeignKey("ProjectId");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyProjectSettings", b =>
                {
                    b.HasOne("GenAiPoc.Contracts.Models.CodeBuddyProjects", "Project")
                        .WithOne("ProjectSettings")
                        .HasForeignKey("GenAiPoc.Contracts.Models.CodeBuddyProjectSettings", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("GenAiPoc.Core.Entities.JiraComponentAnalytics", b =>
                {
                    b.HasOne("GenAiPoc.Core.Entities.JiraAnalytics", "jiraAnalytics")
                        .WithMany()
                        .HasForeignKey("jiraAnalyticsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("jiraAnalytics");
                });

            modelBuilder.Entity("GenAiPoc.Contracts.Models.CodeBuddyProjects", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("ProjectSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
