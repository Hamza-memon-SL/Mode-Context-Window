using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Request.Jira;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IBacklogBuddyService
    {
        Task<GetAllDevopsProjectsResponse> GetAllDevopsProjects(GetAllDevopsProjectsDTO request);
        Task<ImportAllDevopsUserStoriesResponse> ImportAllDevopsUserStories(ImportAllDevopsUserStoriesDTO request);
        Task<GetAllJiraProjectsResponse> GetAllJiraProjects(GetAllJiraProjectsDTO request);
        Task<ImportAllJiraUserStoriesResponse> ImportAllJiraUserStories(ImportAllJiraUserStoriesDTO request);
        Task<GetAllBacklogBuddyProjectsResponse> GetAllBacklogBuddyProjects(string authToken);
        Task<GetAllBacklogBuddyUserStoriesResponse> GetAllBacklogBuddyUserStories(string authToken, string projectId, string projectType);
        Task<ImportAllDevopsUserStoriesResponse> SyncAllDevopsUserStories(SyncAllDevopsUserStoriesDTO request);
        Task<ImportAllJiraUserStoriesResponse> SyncAllJiraUserStories(SyncAllJiraUserStoriesDTO request);
        //Task<HealthAnalyticsResponse> GetProjectHealth(string authToken, string projectId, string projectType);
        Task<GetHealthAnalyticsResponse> GetProjectHealth(string authToken, string projectId, string projectType);
       // Task<UserStoryLevelHealthResponse> GetUserStoryLevelHealth(string authToken, string projectId, string projectType);
        Task<UserStoryLevelHealthResponse> GetUserStoryLevelHealth(string authToken, string projectId, string projectType, int pageNumber = 1, int pageSize = 10);
        Task<Response<JiraAnalyticsResponse>> GetJiraSementicResults(JiraSementicRequest request);

        Task<Core.Response.Response<dynamic>> GetJiraAnalyticsSummaryAsync(string authToken);
        Task<Core.Response.Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryAsync(JiraAnalyticsHistoryRequest request);
        Task<Core.Response.Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryForExtensionAsync(JiraAnalyticsHistoryRequest request);
        Task<Core.Response.Response<bool>> GetJiraAnalyticsTimeSavedAsync(JiraAnalyticsTimeSavedRequest request);

        Task<Core.Response.Response<JiraTreeRootResponse>> GetAllJiraArtifactsTreeView(bool sync);

        Task<Core.Response.Response<bool>> DeleteBacklogBuddyProjectsAsync(string authToken, string projectId);

        Task<Core.Response.Response<CreateJiraStoryResponse>> CreateJiraStoryAsync(CreateJiraStoryRequest request);
        Task<Core.Response.Response<CreateJiraTestCaseResponse>> CreateJiraTestCaseAsync(CreateJiraTestCaseRequest request);

        Task<Core.Response.Response<GenerateJiraArtifactResponse>> GenerateJiraArtifactAsync(GenerateJiraArtifactRequest request);
        Task<Core.Response.Response<ProjectCrudAnalysis>> GetProjectsWithCrudAnalysis(string authToken, string projectId);
        Task<ResponseList<CrudAnalysis>> GetUserLevelCrudAnalysis(string authToken, string projectId);
        Task<ResponseList<FunctionalSize>> GetUserLevelFunctionalSizeAnalysis(string authToken, string projectId);
        Task<Core.Response.Response<FunctionalSizeProjectResponse>> GetProjectsWithFunctionalSizeAnalysis(string authToken, string projectId);
        Task<ResponseList<BacklogAcceptanceCriterianResponse>> GetUserLevelAcceptanceCriterianAnalysis(string authToken, string projectId);
        Task<Core.Response.Response<ProjectLevelAcceptanceCriteriaResponse>> GetProjectsWithAcceptanceCriteriaAnalysis(string authToken, string projectId);

    }
}
