using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Request.Jira;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using Refit;

namespace GenAiPoc.Core.Interfaces.IService.IVisionetClientService
{
    public interface IVisionetClientService
    {
        [Post("/ai-get-models-for-user-group")]
        Task<GetUserGroupModelsResponse> GetAllDynamicModelsAsync([Body] GetDynamicModelRequests getDynamicModelsRequests);

        [Post("/ai-get-models-for-all-user-groups")]
        Task<GetAllUserGroupModelsResponse> GetAllUserGroupDynamicModelsAsync([Body] GetModelsForAllGroupsRequest getDynamicModelsRequests);

        [Post("/v2/ai-get-models-for-all-user-groups")]
        Task<GetAllUserGroupModelsResponse> GetAllUserGroupDynamicModelsV2Async([Body] GetModelsForAllGroupsByUsernameRequest getDynamicModelsRequests);


        [Post("/ssa/process")]
        Task<JiraAnalyticsResponse> GetJiraSementicResultsAsync([Body] JiraPayloadRequest request);

        [Get("/ssa/tree_structure")]
        Task<JiraTreeRootResponse> GetAllJiraArtifactsTreeView();

        [Post("/rest/api/2/issue")]
        Task<CreateJiraStoryResponse> CreateJiraUserStory([Body] CreateJiraStoryRequest request);

        [Post("/rest/api/2/issue")]
        Task<CreateJiraTestCaseResponse> CreateJiraTestCase([Body] CreateJiraTestCaseRequest request);

        [Post("/ssa/generate_data")]
        Task<GenerateJiraArtifactResponse> GenerateJiraArtifact([Body] GenerateJiraArtifactRequest request);


        [Post("/api/interviewQGeneration")]
        Task<Core.Response.Response> AssignUserQuestions([Body] AssignQuestionsDto request);

        [Post("/v2/um-get-user-group")]
        Task<UserGroupResponseRoot> GetUserGroupsAsync([Body] UserGroupRequest request);


        [Post("/api/QueryChatStream")]
        Task<Stream> QueryChatStream([Body] ChatHistoryRequest request);



    }
}
