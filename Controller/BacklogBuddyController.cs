using AutoMapper;
using GenAiPoc.Application.Services;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Request.Jira;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using GenAiPoc.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GenAiPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class BacklogBuddyController : Controller
    {
        private readonly IBacklogBuddyService _backlogBuddyService;
        private readonly IChatHistoryService _chatService;
        private readonly IMapper _mapper;
        public BacklogBuddyController(IBacklogBuddyService backlogBuddyService, IMapper mapper, IChatHistoryService chatService)
        {
            _backlogBuddyService = backlogBuddyService;
            _chatService = chatService;
            _mapper = mapper;
        }
        [HttpPost("GetAllDevopsProjects")]
        public async Task<GetAllDevopsProjectsResponse> GetAllDevopsProjects(GetAllDevopsProjectsDTO request)
        {
            var response = await _backlogBuddyService.GetAllDevopsProjects(request);
            return response;
        }
        [HttpPost("ImportAllDevopsUserStories")]
        public async Task<ImportAllDevopsUserStoriesResponse> ImportAllDevopsUserStories(ImportAllDevopsUserStoriesDTO request)
        {
            var response = await _backlogBuddyService.ImportAllDevopsUserStories(request);
            return response;
        }
        [HttpPost("GetAllJiraProjects")]
        public async Task<GetAllJiraProjectsResponse> GetAllJiraProjects(GetAllJiraProjectsDTO request)
        {
            var response = await _backlogBuddyService.GetAllJiraProjects(request);
            return response;
        }
        [HttpPost("ImportAllJiraUserStories")]
        public async Task<ImportAllJiraUserStoriesResponse> GetAllImportAllJiraUserStories(ImportAllJiraUserStoriesDTO request)
        {
            var response = await _backlogBuddyService.ImportAllJiraUserStories(request);
            return response;
        }
        [HttpGet("GetAllBacklogBuddyProjects")]
        public async Task<GetAllBacklogBuddyProjectsResponse> GetAllBacklogBuddyProjects(string authToken)
        {
            var response = await _backlogBuddyService.GetAllBacklogBuddyProjects(authToken);
            return response;
        }
        [HttpGet("GetAllBacklogBuddyUserStories")]
        public async Task<GetAllBacklogBuddyUserStoriesResponse> GetAllBacklogBuddyUserStories(string authToken,string projectId,string projectType)
        {
            var response = await _backlogBuddyService.GetAllBacklogBuddyUserStories(authToken,projectId,projectType);
            return response;
        }
        [HttpPost("SyncAllDevopsUserStories")]
        public async Task<ImportAllDevopsUserStoriesResponse> SyncAllDevopsUserStories(SyncAllDevopsUserStoriesDTO request)
        {
            var response = await _backlogBuddyService.SyncAllDevopsUserStories(request);
            return response;
        }
        [HttpPost("SyncAllJiraUserStories")]
        public async Task<ImportAllJiraUserStoriesResponse> SyncAllJiraUserStories(SyncAllJiraUserStoriesDTO request)
        {
            var response = await _backlogBuddyService.SyncAllJiraUserStories(request);
            return response;
        }
        [HttpGet("GetProjectHealth")]
        public async Task<GetHealthAnalyticsResponse> GetProjectHealth(string authToken, string projectId, string projectType)
        {
            var response = await _backlogBuddyService.GetProjectHealth(authToken, projectId, projectType);
            return response;
        }
        [HttpGet("GetUserStoryLevelHealth")]
        public async Task<UserStoryLevelHealthResponse> GetUserStoryLevelHealth(string authToken, string projectId, string projectType)
        {
            var response = await _backlogBuddyService.GetUserStoryLevelHealth(authToken, projectId, projectType);
            return response;
        }

        [HttpPost("GetJiraSemanticResult")]
        public async Task<Response<JiraAnalyticsResponse>> GetJiraSemanticResult([FromBody] JiraSementicRequest request) 
        {
            var response = await _backlogBuddyService.GetJiraSementicResults(request);
            return response;
        }

        [HttpPost("GetJiraAnalyticsSummary")]
        public async Task<Response<dynamic>> GetJiraAnalyticsSummaryAsync([FromBody] AuthTokenRequest request)
        {
            var response = await _backlogBuddyService.GetJiraAnalyticsSummaryAsync(request.AuthToken);
            return response;
        }

        [HttpPost("GetJiraAnalyticsHistory")]
        public async Task<Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryAsync([FromBody] JiraAnalyticsHistoryRequest request)
        {
            var response = await _backlogBuddyService.GetJiraAnalyticsHistoryAsync(request);
            return response;
        }

        [HttpPost("GetJiraAnalyticsHistoryForExtension")]
        public async Task<Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryForExtensionAsync([FromBody] JiraAnalyticsHistoryRequest request)
        {
            var response = await _backlogBuddyService.GetJiraAnalyticsHistoryForExtensionAsync(request);
            return response;
        }


        [HttpPost("JiraAnalyticsCodeCoppied")]
        public async Task<Response<bool>> GetJiraAnalyticsTimeSavedAsync([FromBody] JiraAnalyticsTimeSavedRequest request)
        {
            var response = await _backlogBuddyService.GetJiraAnalyticsTimeSavedAsync(request);
            return response;
        }

        [HttpGet("GetJiraArtifactsTreeView")]
        public async Task<Response<JiraTreeRootResponse>> GetJiraTreeViewResult(bool sync)
        {
            var response = await _backlogBuddyService.GetAllJiraArtifactsTreeView(sync);
            return response;
        }

        [HttpPost("CreateJiraUserStory")]
        public async Task<Response<CreateJiraStoryResponse>> GetJiraTreeViewResult([FromBody] CreateJiraStoryRequest request)
        {
            var response = await _backlogBuddyService.CreateJiraStoryAsync(request);
            return response;
        }

        [HttpPost("CreateJiraTestCase")]
        public async Task<Response<CreateJiraTestCaseResponse>> CreateJiraTestCaseAsync([FromBody] CreateJiraTestCaseRequest request)
        {
            var response = await _backlogBuddyService.CreateJiraTestCaseAsync(request);
            return response;
        }

        [HttpGet("DeleteBacklogBuddyProjects")]
        public async Task<Response<bool>> DeleteBacklogBuddyProjectsAsync(string authToken, string projectId)
        {
            var response = await _backlogBuddyService.DeleteBacklogBuddyProjectsAsync(authToken, projectId);
            return response;
        }

        [HttpPost("GenerateJiraArtifact")]
        public async Task<Response<GenerateJiraArtifactResponse>> GenerateJiraArtifactAsync([FromBody] GenerateJiraArtifactRequest request)
        {
            var response = await _backlogBuddyService.GenerateJiraArtifactAsync(request);
            return response;
        }


        [HttpPost("GetCrudProjectLevelHealth")]
        public async Task<Response<ProjectCrudAnalysis>> GetCrudProjectHealth([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetProjectsWithCrudAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }

        [HttpPost("GetCrudUserLevelHealth")]
        public async Task<ResponseList<CrudAnalysis>> GetCrudUserLevelHealth([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetUserLevelCrudAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }

        [HttpPost("GetUserLevelFunctionalSize")]
        public async Task<ResponseList<FunctionalSize>> GetUserLevelFunctionalSize([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetUserLevelFunctionalSizeAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }

        [HttpPost("GetProjectLevelFunctionalSize")]
        public async Task<Response<FunctionalSizeProjectResponse>> GetProjectLevelFunctionalSize([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetProjectsWithFunctionalSizeAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }

        [HttpPost("GetUserLevelAcceptanceCriteria")]
        public async Task<ResponseList<BacklogAcceptanceCriterianResponse>> GetUserLevelAcceptanceCriteria([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetUserLevelAcceptanceCriterianAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }

        [HttpPost("GetProjectLevelAcceptanceCriteria")]
        public async Task<Response<ProjectLevelAcceptanceCriteriaResponse>> GetProjectLevelAcceptanceCriteria([FromBody] DevopsCrudUserLevelRequest request)
        {
            var response = await _backlogBuddyService.GetProjectsWithAcceptanceCriteriaAnalysis(request.AuthToken, request.ProjectId);
            return response;
        }
    }
}