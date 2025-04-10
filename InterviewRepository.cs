using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Interviews;
using GenAiPoc.Core.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Infrastructure.Repository
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly DbContextGenAiPOC _dbContext;

        public InterviewRepository(DbContextGenAiPOC dbContextGenAiPOC)
        {
            this._dbContext = dbContextGenAiPOC;
        }
        public async Task<int> CreateInterviewGroupAsync(string authToken, string name)
        {
            try
            {
                var group = new InterviewGroup
                {
                    Name = name,
                    CreatedBy = authToken,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                };
                _dbContext.InterviewGroups.Add(group);
                await _dbContext.SaveChangesAsync();
                return group.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<List<int>> SaveInterviewGroupResumesAsync(string authToken, int groupId, List<string> filePaths)
        {
            try
            {
                var resumes = filePaths.Select(path => new InterviewGroupResume { GroupId = groupId, Path = path, CreatedBy = authToken }).ToList();
                _dbContext.InterviewGroupResumes.AddRange(resumes);
                await _dbContext.SaveChangesAsync();
                return resumes.Select(r => r.Id).ToList();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        public async Task<bool> UpdateInterviewGroupAsync(UpdateInterviewGroupDto updateInterview, List<string> paths)
        {
            try
            {
                var group = await _dbContext.InterviewGroups
                    .FirstOrDefaultAsync(g => g.Id == updateInterview.groupId && g.CreatedBy == updateInterview.authToken);
                if (group == null) return false;

                group.Name = updateInterview.name;
                group.ModifiedDate = DateTime.UtcNow;

                // Delete specified resumes
                if (updateInterview.deleteResumeIds != null && updateInterview.deleteResumeIds.Any())
                {
                    var resumesToDelete = await _dbContext.Set<InterviewGroupResume>()
                        .Where(r => updateInterview.deleteResumeIds.Contains(r.Id) && r.GroupId == group.Id)
                        .ToListAsync();

                    _dbContext.Set<InterviewGroupResume>().RemoveRange(resumesToDelete);
                }

                // Add new resume paths if provided
                if (paths != null && paths.Any())
                {
                    var newResumes = paths.Select(path => new InterviewGroupResume
                    {
                        GroupId = group.Id,
                        Path = path,
                        CreatedBy = updateInterview.authToken,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = true
                    }).ToList();

                    await _dbContext.Set<InterviewGroupResume>().AddRangeAsync(newResumes);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        //public async Task<List<InterviewGroupDto>> GetAllInterviewGroupsAsync(string authToken, int pageNumber, int pageSize, string? searchQuery)
        //{
        //    var query = _dbContext.InterviewGroups
        //        .Include(x => x.InterviewGroupResumes)
        //        .Where(g => g.CreatedBy == authToken);

        //    if (!string.IsNullOrEmpty(searchQuery))
        //    {
        //        query = query.Where(g => g.Name.Contains(searchQuery));
        //    }

        //    var interviewGroups = await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(g => new InterviewGroupDto
        //        {
        //            Id = g.Id,
        //            Name = g.Name,
        //            TotalResumes = g.InterviewGroupResumes.Count(x=>x.IsDirty == false || x.IsDirty == null),
        //            LlmInteractedCount = g.InterviewGroupResumes.Count(r => r.IsLlmInteracted == true && r.IsDirty == false),
        //            CreatedDate = g.CreatedDate,
        //            ModifiedDate = g.ModifiedDate,
        //            IsActive = g.IsActive,
        //            CreatedBy = g.CreatedBy,
        //            Paths = g.InterviewGroupResumes.Select(x => x.Path).ToList()
        //        })
        //        .ToListAsync();

        //    // Calculate percentage after fetching data
        //    foreach (var group in interviewGroups)
        //    {
        //        group.Percentage = group.TotalResumes > 0
        //            ? (double)group.LlmInteractedCount / group.TotalResumes * 100
        //            : 0;
        //    }

        //    return interviewGroups;
        //}

        public async Task<(List<InterviewGroupDto> Data, int TotalCount)> GetAllInterviewGroupsWithTotalCountAsync(string authToken, int pageNumber, int pageSize, string? searchQuery)
        {
            var query = _dbContext.InterviewGroups
                .Include(x => x.InterviewGroupResumes)
                .Where(g => g.CreatedBy == authToken);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(g => g.Name.Contains(searchQuery));
            }

            var totalCount = await query.CountAsync(); // Get total count in the same query

            var interviewGroups = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new InterviewGroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    TotalResumes = g.InterviewGroupResumes.Count(x => x.IsDirty == false || x.IsDirty == null),
                    LlmInteractedCount = g.InterviewGroupResumes.Count(r => r.IsLlmInteracted == true && r.IsDirty == false),
                    CreatedDate = g.CreatedDate,
                    ModifiedDate = g.ModifiedDate,
                    IsActive = g.IsActive,
                    CreatedBy = g.CreatedBy,
                    Paths = g.InterviewGroupResumes.Where(r => r.IsLlmInteracted == true && r.IsDirty == false).Select(x => x.Path).ToList()
                })
                .ToListAsync();

            // Calculate percentage after fetching data
            foreach (var group in interviewGroups)
            {
                group.Percentage = group.TotalResumes > 0
                    ? (double)group.LlmInteractedCount / group.TotalResumes * 100
                    : 0;
            }

            return (interviewGroups, totalCount);
        }

        public async Task<bool> DeleteInterviewGroupAsync(string authToken, int groupId)
        {
            try
            {
                var group = await _dbContext.InterviewGroups
                    .FirstOrDefaultAsync(g => g.Id == groupId && g.CreatedBy == authToken);
                if (group == null) return false;

                var interviewGroupResumes=  await _dbContext.InterviewGroupResumes
                    .Where(x => x.GroupId == groupId)
                    .ToListAsync();

                if (interviewGroupResumes.Any())
                {
                    _dbContext.Set<InterviewGroupResume>().RemoveRange(interviewGroupResumes);
                }
                _dbContext.InterviewGroups.Remove(group);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<InterviewGroupResume>> GetResumesByGroupIdAsync(string authToken, int groupId)
        {
            return await _dbContext.InterviewGroupResumes
                .Where(r => r.GroupId == groupId && r.CreatedBy == authToken)
                .ToListAsync();
        }

        public async Task<List<ResumeData>> GetResumesPathsByGroupIdAsync(int groupId)
        {
            return await _dbContext.InterviewGroupUserDetails
                .Where(r => r.InterviewGroupId == groupId )
                .Select(x => new ResumeData
                {
                    Path = x.Path,
                    UserId = x.Id
                } )
                .ToListAsync();
        }

        public async Task<List<InterviewGroupUserDetail>> GetAllPaginatedAsync(int pageNumber, int pageSize, string authToken)
        {
            var query = _dbContext.Set<InterviewGroupUserDetail>()
                .Where(x => x.IsActive == true)
                .AsQueryable();
            
            var data = await query
                .Where(x=>x.CreatedBy == authToken)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return data;
        }

        public async Task<List<InterviewGroupUserDetail>> GetAllPaginatedByGroupIdAsync(int groupId, int pageNumber, int pageSize)
        {
            var query = _dbContext.Set<InterviewGroupUserDetail>()
                .Where(x => x.InterviewGroupId == groupId)
                .AsQueryable();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return data;
        }

        public async Task<(List<InterviewGroupUserDetail> Data, int TotalCount)> GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(int groupId, int pageNumber, int pageSize)
        {
            var query = _dbContext.Set<InterviewGroupUserDetail>()
                .Where(x => x.InterviewGroupId == groupId)
                .AsQueryable();

            var totalCount = await query.CountAsync(); // Get total count in the same query
            var pagedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedData, totalCount);
        }



        public async Task<List<InterviewGroupUserQuestion>> GetAllUserQuestions(string authToken, int userGroupId, int pageNumber, int pageSize)
        {
            var query = _dbContext.InterviewGroupUserQuestions
                .Where(q => q.CreatedBy == authToken && q.InterviewGroupUserDetailId == userGroupId);

            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return items ?? new List<InterviewGroupUserQuestion> ();
        }

        //public async Task<bool> DeleteInterviewGroupResumeAsync(List<int> resumeIds,string authToken)
        //{
        //    try
        //    {
        //        var resume = await _dbContext.InterviewGroupResumes
        //                    .Where(x => x.CreatedBy == authToken && x.Id == resumeId)
        //                    .FirstOrDefaultAsync();

        //        if (resume == null) return false;

        //        var userDetails = await _dbContext.InterviewGroupUserDetails
        //            .FirstOrDefaultAsync(u => u.Path == resume.Path);

        //        if (userDetails != null)
        //        {
        //            var userQuestions = await _dbContext.InterviewGroupUserQuestions
        //                .Where(q => q.InterviewGroupUserDetailId == userDetails.Id)
        //                .ToListAsync();

        //            _dbContext.InterviewGroupUserQuestions.RemoveRange(userQuestions);
        //            _dbContext.InterviewGroupUserDetails.Remove(userDetails);
        //        }

        //        _dbContext.InterviewGroupResumes.Remove(resume);
        //        await _dbContext.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error deleting interview group resume: {ex.Message}");
        //        return false;
        //    }
        //}

        public async Task<bool> DeleteInterviewGroupResumeAsync(List<int> resumeIds, string authToken)
        {
            try
            {
                var resumes = await _dbContext.InterviewGroupResumes
                    .Where(x => x.CreatedBy == authToken && resumeIds.Contains(x.Id))
                    .ToListAsync();

                if (!resumes.Any()) return false;

                var paths = resumes.Select(r => r.Path).ToList();

                var userDetails = await _dbContext.InterviewGroupUserDetails
                    .Where(u => paths.Contains(u.Path))
                    .ToListAsync();

                var userDetailIds = userDetails.Select(ud => ud.Id).ToList();

                var userQuestions = await _dbContext.InterviewGroupUserQuestions
                    .Where(q => userDetailIds.Contains(q.InterviewGroupUserDetailId ?? 0))
                    .ToListAsync();

                _dbContext.InterviewGroupUserQuestions.RemoveRange(userQuestions);
                _dbContext.InterviewGroupUserDetails.RemoveRange(userDetails);
                _dbContext.InterviewGroupResumes.RemoveRange(resumes);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting interview group resumes: {ex.Message}");
                return false;
            }
        }

        public async Task CreateInterviewGroupUserQuestionsAsync(List<InterviewGroupUserQuestion> requests)
        {
             await _dbContext.InterviewGroupUserQuestions.AddRangeAsync(requests);
        }
    }
}
