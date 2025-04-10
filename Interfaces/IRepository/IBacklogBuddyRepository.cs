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

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface IBacklogBuddyRepository
    {
        Task<(bool isSuccess, List<AzureDevopsProject>)> GetAllDevopsProjectsAsync(GetAllDevopsProjectsDTO request, string organization);
        Task<string> GetRepositoryInfoFromUrl(string url);
        Task<bool> AddDevopsUserAuthentication(GetAllDevopsProjectsDTO request);
        Task<UserAuthentication> GetUserAuthentication(UserAuthenticationDTO request);
        Task<UserAuthentication> GetUserAuthenticationDetailsByKey(string key);
        Task<BacklogBuddyProjects> CheckIfBacklogBuddyProjectExist(ImportAllDevopsUserStoriesDTO request);
        Task<JiraUserStories> CheckIfBacklogBuddyJiraUserStoryExist(ImportAllDevopsUserStoriesDTO request);
        Task<DevopsUserStories> CheckIfBacklogBuddyDevopsUserStoryExist(ImportAllDevopsUserStoriesDTO request);
        Task<bool> AddBackLogBuddyProject(ImportAllDevopsUserStoriesDTO request);
        Task<(bool isSuccess, DevopsUserStoryResponse)> GetUserStoryFromDevopsAsync(ImportAllDevopsUserStoriesDTO request, UserAuthentication user);
        Task<bool> AddDevopsUserStoriesInDb(ImportAllDevopsUserStoriesDTO request, DevopsUserStoryResponse response, UserAuthentication user);
        Task<List<DevopsUserStories>> GetDevopsUserStoriesFromDb(ImportAllDevopsUserStoriesDTO request);
        Task<(bool isSuccess, List<JiraProject>)> GetAllJiraProjectsAsync(GetAllJiraProjectsDTO request);
        Task<bool> AddJiraUserAuthentication(GetAllJiraProjectsDTO request);
        Task<List<JiraUserStories>> GetJiraUserStoriesFromDb(ImportAllJiraUserStoriesDTO request);
        Task<(bool isSuccess, JiraUserStoryResponse)> GetUserStoryFromJiraAsync(ImportAllJiraUserStoriesDTO request, UserAuthentication user);
        Task<bool> AddJiraUserStoriesInDb(ImportAllJiraUserStoriesDTO request, JiraUserStoryResponse response, UserAuthentication user);
        //Task<List<BacklogBuddyProjects>> GetAllBacklogBuddyProjectsAsync(string authToken);
        //Task<(List<BacklogBuddyProjects>, int)> GetAllBacklogBuddyProjectsAsync(string authToken);
        Task<List<ProjectResponse>> GetAllBacklogBuddyProjectsAsync(string authToken);
        Task<List<DevopsUserStories>> GetAllBacklogBuddyDevopsUserStoriesAsync(string authToken, string projectId);
        Task<List<JiraUserStories>> GetAllBacklogBuddyJiraUserStoriesAsync(string authToken, string projectId);
        Task<(bool isSuccess, List<DevopsUserStories>)> SyncDevopsUserStories(SyncAllDevopsUserStoriesDTO request, DevopsUserStoryResponse response, UserAuthentication user);
        Task<(bool isSuccess, DevopsUserStoryWIQLResponse)> GetAllDevopsUserStoriesAsync(SyncAllDevopsUserStoriesDTO request, UserAuthentication user);
        Task<(bool isSuccess, DevopsUserStoryResponse)> GetUserStoryFromDevopsAsync(SyncAllDevopsUserStoriesDTO request, UserAuthentication user);

        Task<(bool isSuccess, JiraUserStoryResponse)> GetUserStoryFromJiraAsync(SyncAllJiraUserStoriesDTO request, UserAuthentication user);
        Task<(bool isSuccess, List<JiraUserStories>)> SyncJiraUserStories(SyncAllJiraUserStoriesDTO request, JiraUserStoryResponse response, UserAuthentication user);
        // Task<(bool isSuccess, List<DevopsUserStories>)> SyncDevopsUserStories(ImportAllDevopsUserStoriesDTO request, DevopsUserStoryResponse response, UserAuthentication user);

        Task<HealthAnalyticsResponse> GetBacklogBuddyDevopsProjectHealth(string authToken,string projectId);
        Task<HealthAnalyticsResponse> GetBacklogBuddyJiraProjectHealth(string authToken, string projectId);
        Task<UserStoryHealthAnalyticsResponse> GetBacklogBuddyDevopsUserStoryHealth(string authToken, string projectId, int pageNumber, int pageSize);
        Task<UserStoryHealthAnalyticsResponse> GetBacklogBuddyJiraUserStoryHealth(string authToken, string projectId, int pageNumber, int pageSize);
        //Task<HealthAnalyticsResponse> GetBacklogBuddyJiractHealth(string authToken, string projectId);

        Task AddOrUpdateJiraAnalyticsAsync(JiraAnalyticsResponse jiraAnalyticsResponse, JiraSementicRequest jiraSementicRequest);
        Task<object> GetAnalyticsSummaryAsync(string authToken);

        Task<List<JiraSearchAnalytics>> GetJiraAnalyticsHistoryWithFilterAsync(JiraAnalyticsHistoryRequest request);

        Task GetJiraAnalyticsTimeSavedAsync(JiraAnalyticsTimeSavedRequest request);

        Task<bool> DeleteBacklogBuddyProjectsAsync(string authToken, string projectId);
        Task<JiraSearchAnalyticsHistoryResponse> GetJiraSearchAnalyticsPaginatedAsync(JiraAnalyticsHistoryRequest request);
        Task<JiraSearchAnalyticsHistoryResponse> GetJiraSearchAnalyticsPaginatedExtensionAsync(JiraAnalyticsHistoryRequest request);
        Task<string> GetJiraArtifactsTree();
        Task<bool> SyncJiraArtifacts(JiraSync jiraSync);

        Task<List<DevopsUserStories>> GetDevOpsUserStories(string authToken);
        Task<List<DevopsUserStories>> GetDevOpsUserStoriesByProject(string authToken, string projectId);

    }
}
