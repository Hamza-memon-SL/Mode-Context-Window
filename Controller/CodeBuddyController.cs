using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Mvc;
using GenAiPoc.DTOs;
using GenAiPoc.Core.DTOs;
using AutoMapper;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Application.Services;
using GenAiPoc.Core.Request;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Authorization;

namespace GenAiPoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class CodeBuddyController : Controller
    {
        private readonly ICodeBuddyService _codeBuddyService;
        private readonly IChatHistoryService _chatService;
        private readonly IAIModelService _aiModelService;
        private readonly IMapper _mapper;
        public CodeBuddyController(ICodeBuddyService codeBuddyService, IMapper mapper, IChatHistoryService chatService, IAIModelService aiModelService)
        {
            _codeBuddyService = codeBuddyService;
            _chatService = chatService;
            _mapper = mapper;
            _aiModelService = aiModelService;
        }

        [HttpPost("ImportURL")]

        public async Task<ImportResponse> ImportFile([FromBody] ImportFileDTO request)
        {
            var response = await _codeBuddyService.ImportFile(request);
            return response;
        }

        [HttpPost("CreateProject")]

        public async Task<ImportResponse> CreateProject([FromBody] ImportFileDTO request)
        {
            var response = await _codeBuddyService.CreateProject(request);
            return response;
        }

        [HttpPost("DeleteWorkspace")]

        public async Task<DeleteWorkspaceResponse> DeleteWorkspace(int projectId)
        {
            var response = await _codeBuddyService.DeleteWorkspace(projectId);
            return response;
        }

        [HttpPost("EditWorkspace")]
        public async Task<EditWorkspaceResponse> EditWorkspace(EditWorkspaceDTO request)
        {
            var response = await _codeBuddyService.EditWorksapce(request);
            return response;
        }
        [HttpGet("GetWorkspace")]
        public async Task<GetWorkspaceResponse> GetWorkspace(int ProjectId)
        {
            var response = await _codeBuddyService.GetWorkspaceAsync(ProjectId);
            return response;
        }

        [HttpGet("GenerateDocument")]
        public async Task<IActionResult> GenerateCodeAnalysisReport(int projectId)
        {
            var (pdfBytes, projectName) = await _codeBuddyService.GetAllIndexedFilesAsync(projectId);
            return File(pdfBytes, "application/pdf", $"{projectName}_CodeAnalysis.pdf");
        }

        [HttpGet("GetWorkspaceDetailsByAuthToken")]
        public async Task<GetWorkspaceDetailsByAuthTokenResponse> GetWorkspaceDetailsByAuthToken(string authToken)
        {
            var response = await _codeBuddyService.GetWorkspaceDetailsAsync(authToken);
            return response;
        }


        [HttpGet("GetAllProjects")]
        public async Task<GetAllProjectsResponse> GetAllProjects(string token)
        {
            var response = await _codeBuddyService.GetAllProjectsAsync(token);
            return response;
        }

        [HttpGet("GetModels")]
        public async Task<GetModelsResponse> GetModels()
        {
            var response = await _codeBuddyService.GetModelsAsync();
            return response;
        }

        [HttpGet("GetWorkspaceDetails")]
        public async Task<GetWorkspaceDetailsResponse> GetWorkspaceDetails(int itemId, string itemType)
        {
            var response = await _codeBuddyService.GetWorkspaceDetailsAsync(itemId, itemType);
            return response;
        }
        [HttpPost("SyncWorkspace")]
        public async Task<SyncWorkspaceResponse> SyncWorkspace(SyncWorkspaceDTO request)
        {
            var response = await _codeBuddyService.SyncWorkspace(request);
            return response;
        }

        [HttpGet("GetChatHistoryByAuthToken")]
        public async Task<GetChatHistoryResponse> GetChatHistoryByAuthToken(string authToken, int? historyChatType)
        {
            var response = await _chatService.GetChatHistoryByAuthToken(authToken, historyChatType);
            return response;
        }
        [HttpGet("GetChatHistoryByAuthTokenV2")]
        public async Task<GetSessionChatHistoryResponse> GetSessionChatHistoryByAuthToken(string authToken, int? historyChatType)
        {
            var response = await _chatService.GetSessionChatHistoryByAuthToken(authToken, historyChatType);
            return response;
        }
        [HttpGet("GetChatHistoryByWorkspaceId")]
        public async Task<GetChatHistoryResponse> GetChatHistoryByWorkspaceId(int workSpaceId)
        {
            var response = await _chatService.GetChatHistoryByWorkspaceId(workSpaceId);
            return response;
        }
        [HttpGet("GetChatPrompt")]
        public async Task<GetChatPromptsResponse> GetChatPrompt()
        {
            var response = await _chatService.GetChatPrmoptsAsync();
            return response;
        }
        [HttpPost("InviteProject")]
        public async Task<InviteProjectResponse> InviteProject(InviteProjectDTO request)
        {
            var response = await _codeBuddyService.InviteProjectAsync(request);
            return response;
        }
        [HttpPost("AcceptInvite")]
        public async Task<InviteAcceptResponse> AcceptInvite([FromQuery] string inviteToken, [FromQuery] string? authToken)
        {
            var response = await _codeBuddyService.InviteAcceptAsync(inviteToken, authToken);
            return response;
        }

        [HttpPost("CodeCoppied")]
        public async Task<bool> CodeCoppied([FromBody] CodeSuggestionDTO codeSuggestion, [FromQuery] string? authToken)
        {
            codeSuggestion.AuthToken = authToken;
            var response = await _codeBuddyService.CodeCoppied(codeSuggestion);
            return response;
        }

        [HttpGet("GetWorkSpaceStats")]
        public async Task<WorkSpaceStatsResponse> GetWorkSpaceStats([FromQuery] int? projectId, [FromQuery] string? authToken)
        {
            var response = await _codeBuddyService.GetWorkSpaceStats(projectId, authToken);
            return response;
        }

        [HttpGet("GetBoilerPlatesTemplate")]
        public async Task<GetBoilerPlatesTemplatesResponse> GetBoilerPlatesTemplates()
        {
            var response = await _codeBuddyService.GetBoilerPlatesTemplates();
            return response;
        }
        [HttpPost("SubmitFeedback")]
        public async Task<SubmitFeedbackResponse> SubmitFeedback(SubmitFeedbackDTO request)
        {
            var response = await _codeBuddyService.SubmitFeedback(request);
            return response;
        }

        [HttpPost("GetUserSpecificExtensionAnalytics")]
        public async Task<GetUserSpecificExtensionAnalyticsResponse> GetUserSpecificExtensionAnalytics([FromQuery] string? authToken, int? historyChatType)
        {
            var response = await _codeBuddyService.GetUserSpecificExtensionAnalytics(authToken, historyChatType);
            return response;
        }

        [HttpPost("GetCustomAiModels")]
        public async Task<Response<AIModelDTO>> GetAiModels([FromBody] GetAiModelsRequests request)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return new Response<AIModelDTO>
                {
                    Success = false,
                    Message = "Authorization token is missing or invalid."
                };
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();
            request.Token = token;

            var response = await _aiModelService.GetAllAIModelsAsync(request);
            return response;
        }


        [HttpPost("GetDynamicModels")]
        public async Task<Response<GetUserGroupModelsResponse>> GetDynamicModels([FromBody] GetDynamicModelRequests request)
        {

            var response = await _aiModelService.GetAllDynamicModelsAsync(request);
            return response;
        }
        [HttpPost("GetModelsForAllGroups")]
        public async Task<Response<GetAllUserGroupModelsResponse>> GetModelsForAllGroups([FromBody] GetModelsForAllGroupsRequest request)
        {

            var response = await _aiModelService.GetAllUserGroupDynamicModelsAsync(request);
            return response;
        }
        [HttpPost("v2/GetModelsForAllGroups")]
        public async Task<Response<GetAllUserGroupModelsResponse>> GetModelsForAllGroupsV2([FromBody] GetModelsForAllGroupsByUsernameRequest request)
        {

            var response = await _aiModelService.GetAllUserGroupDynamicModelsV2Async(request);
            return response;
        }

        [HttpPost("GetVisitBotStats")]
        public async Task<GetVisiBotStatsResponse> GetVisitBotStats(GetVisitBotStatsRequest request)
        {
            var response = await _codeBuddyService.GetVisitBotStats(request);
            return response;
        }
        [HttpPost("CreateCodeBuddyPerformance")]
        public async Task<CreateCodeBuddyPerformanceResponse> CreateCodeBuddyPerformance(CreateCodeBuddyPerformanceRequest request)
        {
            var response = await _codeBuddyService.CreateCodeBuddyPerformance(request);
            return response;
        }
    }
}
