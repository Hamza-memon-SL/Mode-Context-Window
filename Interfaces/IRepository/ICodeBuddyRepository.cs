using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using GenAiPoc.Contracts.Models;
using Azure.Storage.Blobs.Models;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Response.Jira;
using GenAiPoc.Core.Request;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface ICodeBuddyRepository
    {

        Task<ImportResponse> ImportFile(ImportFileDTO dto, string repoPath, string repositoryId);
        Task<ImportResponse> ImportFileWithoutCheckSum(ImportFileDTO dto, string repoPath, string repositoryId);
        Task<string> GetTempFolderPath();
        Task<string> GetTempFolderPath(string extendedPath);
        Task<CodeBuddyProjects?> GetProjectByRepoUrl(ImportFileDTO dto);
        Task<ImportResponse> ProcessRepositoryAsync(ImportFileDTO dto, string localPath);
        Task<(bool isSuccess, bool isPrivate, bool isBranchExist, string message)> ClonePublicRepository(ImportFileDTO dto, string localPath);
        Task<(bool isSuccess, bool isPrivate, bool isBranchExist, string message)> ClonePrivateRepository(ImportFileDTO dto, string localPath);
        Task<string> GetRepositoryName(string repoUrl);
        Task<bool> UpdateProjectStatus(int projectId);
        Task<List<CodeBuddyFileDetails>> GetAllFileDetails(int projectId, string repoUrl, string localPath);
        Task<bool> SaveFileDetailsToDatabase(List<CodeBuddyFileDetails> fileDetailsList);
        Task DeleteDirectory(string targetDir);
        Task<List<WorkspaceTree>> GetWorkspaceAsync(int ProjectId, string localpath);
        Task<List<WorkspaceDetailsByAuthModel>> GetWorkspaceDetailsAsync(string authToken);
        Task<List<ProjectsModel>> GetAllProjectsAsync(string token);
        Task<List<AIModels>> GetModelsAsync();
        Task<GetWorkspaceModel> GetWorkspaceDetailsAsync(int itemId, string itemType);
        Task<bool> DeleteWorkspaceAsync(int projectId);
        Task<bool> EditWorkspaceAsync(EditWorkspaceDTO request);
        Task<bool> DetectNewOrUpdatedOrDeletedFiles(List<CodeBuddyFileDetails> fileDetailsList, string localRepoPath, int projectId);
        Task<List<ChatHistories>> GetChatHistoryByWorkspaceId(int Id);
        Task<List<ChatHistories>> GetChatHistoryByAuthToken(string AuthToken, int? historyType);
        Task<List<ChatPrompts>> GetChatPrompts();
        Task<bool> InviteProjectByEmail(InviteProjectDTO request);
        Task<bool> InviteAcceptAsyncByURL(string inviteToken, string authToken);
        Task<bool> CodeCoppied(CodeSuggestion codeSuggestion);
        Task<WorkSpaceStatsResponse> GetWorkSpaceStats(int? projectId, string? authToken);
        Task<List<BoilerPlatesTemplate>> GetBoilerPlatesTemplatesAsync();
        Task<bool> SubmitFeedbackAsync(SubmitFeedbackDTO request);
        Task<GetRepositoryIdResponse> IsDevopsRepoExist(ImportFileDTO dto);
        Task<ImportResponse> ProcessDevopsRepository(ImportFileDTO dto, string repositoryId);
        Task<bool> CheckProjectCheckSumIsExist(string localPath, string repoName);
        Task<bool> UpdateProjectCheckSum(string localPath, string repoName, int projectId);
        Task<bool> UpdateWorkSpaceFileType(int projectId);
        Task<bool> CheckDevopsProjectCheckSumIsExist(string url, string _pat, string branchName, string repositoryId);
        Task<bool> DetectDevopsNewOrUpdatedOrDeletedFiles(ImportFileDTO dto, string repositoryId, int projectId);
        Task<bool> UpdateDevopsProjectCheckSum(string url, string _pat, string branchName, string repositoryId, int projectId);
        Task<ExtensionAnalytics> GetUserSpecificExtensionAnalyticsAsync(string authToken, int? historyChatType);
        Task<List<ChatSessionHistory>> GetSessionChatHistoryByAuthToken(string AuthToken, int? historyChatType);
        Task<VisiBotStatsAnalytics> GetVisiBotStats(GetVisitBotStatsRequest request);
        Task<bool> CreateCodeBuddyPerformanceAsync(CreateCodeBuddyPerformanceRequest request);
        Task<List<CodeBuddyFileDetails>> GetAllIndexedFiles(int projectId);
        Task<List<string>> GetRepositoryInfoFromUrl(string url);

        string GetDevopsUrl(List<string> urlInfos);

    }
}
