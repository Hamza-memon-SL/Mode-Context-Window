using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IInterviewService
    {
        Task<Response<int>> CreateInterviewGroupAsync(string authToken, string name);
        Task<ResponseList<int>> CreateInterviewGroupResumesAsync(string authToken, int groupId, List<IFormFile> files);
        Task<Response<bool>> UpdateInterviewGroupAsync(UpdateInterviewGroupDto updateInterviewGroupDto);
        Task<PaginatedResponse<InterviewGroupDto>> GetAllInterviewGroupsAsync(string authToken, int pageNumber, int pageSize, string? searchQuery);
        Task<Response<bool>> DeleteInterviewGroupAsync(string authToken, int groupId);
        Task<ResponseList<InterviewGroupResumeDto>> GetResumesByGroupIdAsync(string authToken, int groupId);

        Task<ResponseList<InterviewGroupUserDetailDTO>> GetAllPaginatedAsync(int pageNumber, int pageSize, string authToken);
        Task<PaginatedResponse<InterviewGroupUserDetailDTO>> GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(int groupId, int pageNumber, int pageSize);
        Task<ResponseList<InterviewGroupUserQuestionDto>> GetAllUserQuestions(string authToken, int userGroupId, int pageNumber, int pageSize);
        Task<Response<bool>> DeleteInterviewGroupResumeAsync(List<int> resumeIds, string authToken);

        Task<Core.Response.Response<bool>> AssignQuestionsAsync(AssignQuestionsDto request);
        Task<Core.Response.Response<bool>> CreateInterviewGroupUserQuestionAsync(List<CreateInterviewGroupUserQuestionDto> requests);
    }
}
