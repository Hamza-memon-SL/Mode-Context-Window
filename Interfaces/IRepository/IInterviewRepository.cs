using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Interviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface IInterviewRepository
    {
        Task<int> CreateInterviewGroupAsync(string authToken, string name);
        Task<List<int>> SaveInterviewGroupResumesAsync(string authToken, int groupId, List<string> filePaths);
        Task<bool> UpdateInterviewGroupAsync(UpdateInterviewGroupDto updateInterview, List<string> paths);
        //Task<List<InterviewGroupDto>> GetAllInterviewGroupsAsync(string authToken, int pageNumber, int pageSize, string? searchQuery);
        Task<(List<InterviewGroupDto> Data, int TotalCount)> GetAllInterviewGroupsWithTotalCountAsync(string authToken, int pageNumber, int pageSize, string? searchQuery);
        Task<bool> DeleteInterviewGroupAsync(string authToken, int groupId);
        Task<List<InterviewGroupResume>> GetResumesByGroupIdAsync(string authToken, int groupId);
        Task<List<InterviewGroupUserDetail>> GetAllPaginatedAsync(int pageNumber, int pageSize, string authToken);
        //Task<List<InterviewGroupUserDetail>> GetAllPaginatedByGroupIdAsync(int groupId, int pageNumber, int pageSize);
        Task<(List<InterviewGroupUserDetail> Data, int TotalCount)> GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(int groupId, int pageNumber, int pageSize);
        Task<List<InterviewGroupUserQuestion>> GetAllUserQuestions(string authToken, int userGroupId, int pageNumber, int pageSize);
        Task<bool> DeleteInterviewGroupResumeAsync(List<int> resumeIds, string authToken);

        Task<List<ResumeData>> GetResumesPathsByGroupIdAsync(int groupId);

        Task CreateInterviewGroupUserQuestionsAsync(List<InterviewGroupUserQuestion> requests);



    }
}
