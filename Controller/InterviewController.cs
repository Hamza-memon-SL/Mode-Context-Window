using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Mvc;

namespace GenAiPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            this._interviewService = interviewService;
        }


        [HttpPost("UploadResumes")]
        public async Task<ResponseList<int>> CreateInterviewGroupResumesAsync([FromForm] UploadResumesDto upload)
        {
            var response = await _interviewService.CreateInterviewGroupResumesAsync(upload.AuthToken, upload.GroupId, upload.files);
            return response;
        }

        [HttpPost("CreateGroup")]
        public async Task<Response<int>> CreateGroupAsync(string authToken, string name)
        {
            var response = await _interviewService.CreateInterviewGroupAsync(authToken, name);
            return response;
        }

        [HttpPut("UpdateGroup")]
        public async Task<Response<bool>> UpdateInterviewGroupAsync(UpdateInterviewGroupDto viewGroupDto)
        {
            var response = await _interviewService.UpdateInterviewGroupAsync(viewGroupDto);
            return response;
        }

        [HttpGet("GetAllInterviewGroups")]
        public async Task<PaginatedResponse<InterviewGroupDto>> GetAllInterviewGroupsAsync(string authToken, int pageNumber, int pageSize, string? searchQuery)
        {
            var response = await _interviewService.GetAllInterviewGroupsAsync(authToken, pageNumber, pageSize, searchQuery);
            return response;
        }

        [HttpDelete("DeleteInterviewGroup")]
        public async Task<Response<bool>> DeleteInterviewGroupAsync(string authToken, int groupId)
        {
            var response = await _interviewService.DeleteInterviewGroupAsync(authToken, groupId);
            return response;
        }

        [HttpGet("GetResumesByGroupId")]
        public async Task<ResponseList<InterviewGroupResumeDto>> GetResumesByGroupIdAsync(string authToken, int groupId)
        {
            var response = await _interviewService.GetResumesByGroupIdAsync(authToken, groupId);
            return response;
        }

        [HttpGet("GetAllUserDetails")]
        public async Task<ResponseList<InterviewGroupUserDetailDTO>> GetAllPaginatedAsync(int pageNumber, int pageSize, string authToken)
        {
            var response = await _interviewService.GetAllPaginatedAsync(pageNumber, pageSize, authToken);
            return response;
        }

        [HttpGet("GetAllUserDetailsByGroupId")]
        public async Task<PaginatedResponse<InterviewGroupUserDetailDTO>> GetAllPaginatedByGroupIdAsync(int groupId, int pageNumber, int pageSize)
        {
            var response = await _interviewService.GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(groupId, pageNumber, pageSize);
            return response;
        }

        [HttpGet("GetAllUserQuestionsByUserId")]
        public async Task<ResponseList<InterviewGroupUserQuestionDto>> GetAllPaginatedAsync(string authToken, int groupUserId, int pageNumber, int pageSize)
        {
            var response = await _interviewService.GetAllUserQuestions(authToken, groupUserId, pageNumber, pageSize);
            return response;
        }

        [HttpDelete("DeleteInterviewGroupResume")]
        public async Task<Response<bool>> DeleteInterviewGroupResumeAsync(string authToken, List<int> resumeIds)
        {
            var response = await _interviewService.DeleteInterviewGroupResumeAsync(resumeIds, authToken);
            return response;
        }

        [HttpPost("AssignQuestions")]
        public async Task<Response<bool>> AssignQuestions([FromBody] AssignQuestionsDto request)
        {
            return await _interviewService.AssignQuestionsAsync(request);
        }

        [HttpPost("createUserQuestions")]
        public async Task<Response<bool>> CreateInterviewGroupUserQuestionAsync([FromBody] List<CreateInterviewGroupUserQuestionDto> request)
        {
            var response = await _interviewService.CreateInterviewGroupUserQuestionAsync(request);
            return response;
        }



    }
}