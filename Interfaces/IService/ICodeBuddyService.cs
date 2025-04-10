using GenAiPoc.Contracts.Models;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface ICodeBuddyService
    {
        Task<GetModelsResponse> GetModelsAsync();
        Task<ImportResponse> ImportFile(ImportFileDTO model);
        Task<ImportResponse> CreateProject(ImportFileDTO model);
        Task<GetWorkspaceResponse> GetWorkspaceAsync(int ProjectId);
        Task<GetAllProjectsResponse> GetAllProjectsAsync(string token);
        Task<GetWorkspaceDetailsResponse> GetWorkspaceDetailsAsync(int itemId, string itemType);
        Task<SyncWorkspaceResponse> SyncWorkspace(SyncWorkspaceDTO request);
        Task<DeleteWorkspaceResponse> DeleteWorkspace(int projectId);
        Task<EditWorkspaceResponse> EditWorksapce(EditWorkspaceDTO request);
        Task<GetWorkspaceDetailsByAuthTokenResponse> GetWorkspaceDetailsAsync(string authToken);
        Task<InviteProjectResponse> InviteProjectAsync(InviteProjectDTO request);
        Task<InviteAcceptResponse> InviteAcceptAsync(string inviteToken, string authToken);
        Task<bool> CodeCoppied(CodeSuggestionDTO codeSuggestionDTO);
        Task<WorkSpaceStatsResponse> GetWorkSpaceStats(int? projectId, string? authToken);
        Task<GetBoilerPlatesTemplatesResponse> GetBoilerPlatesTemplates();
        Task<SubmitFeedbackResponse> SubmitFeedback(SubmitFeedbackDTO request);
        Task<GetUserSpecificExtensionAnalyticsResponse> GetUserSpecificExtensionAnalytics(string? authToken, int? historyChatType);
        Task<GetVisiBotStatsResponse> GetVisitBotStats(GetVisitBotStatsRequest request);
        Task<CreateCodeBuddyPerformanceResponse> CreateCodeBuddyPerformance(CreateCodeBuddyPerformanceRequest request);

        Task<(byte[], string)> GetAllIndexedFilesAsync(int projectId);
    }
}
