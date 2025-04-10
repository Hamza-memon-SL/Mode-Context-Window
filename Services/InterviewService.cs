using Azure;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Interviews;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Interfaces.IService.IVisionetClientService;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using GenAiPoc.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly ILogger<InterviewService> _logger;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IBlobStorageService _blobStorage;
        private readonly IBacklogBuddyRepository _backlogBuddyRepository;
        private IVisionetClientService _visionetClientService;

        public InterviewService(ILogger<InterviewService> logger,
            IInterviewRepository interviewRepository,
            IBlobStorageService blobStorage,
            IBacklogBuddyRepository backlogBuddyRepository,
            IVisionetClientService visionetClientService
            )
        {
            _logger = logger;
            this._interviewRepository = interviewRepository;
            this._blobStorage = blobStorage;
            this._backlogBuddyRepository = backlogBuddyRepository;
            this._visionetClientService = visionetClientService;
        }

        public async Task<Core.Response.Response<int>> CreateInterviewGroupAsync(string authToken, string name)
        {
            Core.Response.Response<int> response = new Core.Response.Response<int>();
            try
            {
                if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(name))
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                var result = await _interviewRepository.CreateInterviewGroupAsync(authToken, name);
                response.Data = result;
                response.Success = result > 0 ? true : false;
                response.Message = result > 0 ? StatusAndMessagesKeys.CreateSuccess : StatusAndMessagesKeys.CreateFailed;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while uploading resumes.";
                response.Success = false;
                return response;
            }
        }

        public async Task<ResponseList<int>> CreateInterviewGroupResumesAsync(string authToken, int groupId, List<IFormFile> files)
        {
            ResponseList<int> response = new ResponseList<int>();
            try
            {
                if (string.IsNullOrEmpty(authToken) || files == null || !files.Any() || groupId <= 0)
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                var uploadedPaths = await _blobStorage.UploadInterviewResumesToBlobAsync(files);
                var result = await _interviewRepository.SaveInterviewGroupResumesAsync(authToken, groupId, uploadedPaths);
                response.Data = result;
                response.Success = result != null ? true : false;
                response.Message = result != null ? StatusAndMessagesKeys.CreateSuccess : StatusAndMessagesKeys.CreateFailed;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while uploading resumes.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Core.Response.Response<bool>> UpdateInterviewGroupAsync(UpdateInterviewGroupDto groupDto)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                List<string> uploadedPaths = new List<string>();
                if (groupDto?.files != null)
                {
                    uploadedPaths = await _blobStorage.UploadInterviewResumesToBlobAsync(groupDto.files);
                }


                var result = await _interviewRepository.UpdateInterviewGroupAsync(groupDto, uploadedPaths);
                response.Data = result;
                response.Success = result;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while updating the interview group.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Core.Response.PaginatedResponse<InterviewGroupDto>> GetAllInterviewGroupsAsync(string authToken, int pageNumber, int pageSize, string? searchQuery)
        {
            try
            {
                var (groups, totalCount) = await _interviewRepository.GetAllInterviewGroupsWithTotalCountAsync(authToken, pageNumber, pageSize, searchQuery);

                return new Core.Response.PaginatedResponse<InterviewGroupDto>
                {
                    Data = new PaginatedResult<InterviewGroupDto>
                    {
                        Items = groups,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    Success = true,
                    Message = groups.Any() ? StatusAndMessagesKeys.GetAllSuccess : StatusAndMessagesKeys.NoDataFound
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving interview groups.");
                return new Core.Response.PaginatedResponse<InterviewGroupDto>
                {
                    Message = "An error occurred while retrieving interview groups.",
                    Success = false,
                    Data = new PaginatedResult<InterviewGroupDto>()
                };
            }
        }


        public async Task<Core.Response.Response<bool>> DeleteInterviewGroupAsync(string authToken, int groupId)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                var result = await _interviewRepository.DeleteInterviewGroupAsync(authToken, groupId);
                response.Data = result;
                response.Success = result;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while deleting the interview group.";
                response.Success = false;
                return response;
            }
        }

        public async Task<ResponseList<InterviewGroupResumeDto>> GetResumesByGroupIdAsync(string authToken, int groupId)
        {
            ResponseList<InterviewGroupResumeDto> response = new ResponseList<InterviewGroupResumeDto>();
            try
            {
                var resumes = await _interviewRepository.GetResumesByGroupIdAsync(authToken, groupId);
                response.Data = resumes.Select(r => new InterviewGroupResumeDto
                {
                    Id = r.Id,
                    Path = r.Path,
                    GroupId = r.GroupId,
                    CreatedDate = r.CreatedDate,
                    ModifiedDate = r.ModifiedDate,
                    IsActive = r.IsActive,
                    CreatedBy = r.CreatedBy
                }).ToList();
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while retrieving resumes.";
                response.Success = false;
                return response;
            }
        }


        public async Task<ResponseList<InterviewGroupUserDetailDTO>> GetAllPaginatedAsync(int pageNumber, int pageSize, string authToken)
        {
            var response = new ResponseList<InterviewGroupUserDetailDTO>();
            try
            {
                var pagedData = await _interviewRepository.GetAllPaginatedAsync(pageNumber, pageSize, authToken);
                response = new ResponseList<InterviewGroupUserDetailDTO>()
                {
                    Data = pagedData
                    .Select(x => new InterviewGroupUserDetailDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Email = x.Email,
                        Path = x.Path,
                        InterviewGroupId = x.InterviewGroupId,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate
                    })
                .ToList(),
                    Message = StatusAndMessagesKeys.GetAllSuccess,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated Interview Group User Details.");
                response.Message = "An error occurred while fetching paginated Interview Group User Details.";
                response.Success = false;
            }

            return response;
        }
        public async Task<Core.Response.PaginatedResponse<InterviewGroupUserDetailDTO>> GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(int groupId, int pageNumber, int pageSize)
        {
            try
            {
                var (pagedData, totalCount) = await _interviewRepository.GetAllUserDetailsPaginatedWithTotalCountByGroupIdAsync(groupId, pageNumber, pageSize);

                return new Core.Response.PaginatedResponse<InterviewGroupUserDetailDTO>
                {
                    Data = new PaginatedResult<InterviewGroupUserDetailDTO>
                    {
                        Items = pagedData.Select(x => new InterviewGroupUserDetailDTO
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Email = x.Email,
                            Path = x.Path,
                            InterviewGroupId = x.InterviewGroupId,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate
                        }).ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    Message = StatusAndMessagesKeys.GetAllSuccess,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated Interview Group User Details by GroupId.");
                return new Core.Response.PaginatedResponse<InterviewGroupUserDetailDTO>
                {
                    Message = "An error occurred while fetching paginated Interview Group User Details by GroupId.",
                    Success = false,
                    Data = new PaginatedResult<InterviewGroupUserDetailDTO>()
                };
            }
        }

        public async Task<ResponseList<InterviewGroupUserQuestionDto>> GetAllUserQuestions(string authToken, int userGroupId, int pageNumber, int pageSize)
        {
            ResponseList<InterviewGroupUserQuestionDto> response = new ResponseList<InterviewGroupUserQuestionDto>();
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    response.Message = "Invalid authentication token.";
                    response.Success = false;
                    return response;
                }

                var result = await _interviewRepository.GetAllUserQuestions(authToken, userGroupId, pageNumber, pageSize);

                var resultDto = result.Select(q => new InterviewGroupUserQuestionDto
                {
                    Id = q.Id,
                    Question = q.Question,
                    InterviewGroupUserDetailId = q.InterviewGroupUserDetailId,
                    DifficultyLevel = q.DifficultyLevel,
                    Agent = q.Agent,
                    ImpactFactor = q.ImpactFactor,
                    CreatedDate = q.CreatedDate
                }).ToList();


                response.Message = result != null ? StatusAndMessagesKeys.GetAllSuccess : StatusAndMessagesKeys.NoDataFound;
                response.Data = resultDto;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated interview group user questions.");
                response.Message = "An error occurred while processing your request.";
                response.Success = false;
            }
            return response;
        }

        public async Task<Core.Response.Response<bool>> DeleteInterviewGroupResumeAsync(List<int> resumeIds, string authToken)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                var isDeleted = await _interviewRepository.DeleteInterviewGroupResumeAsync(resumeIds, authToken);
                if (!isDeleted)
                {
                    response.Message = StatusAndMessagesKeys.NoDataFound;
                    response.Success = false;
                    return response;
                }

                response.Data = true;
                response.Message = StatusAndMessagesKeys.DeleteSuccess;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Interview Group Resume.");
                response.Message = "An error occurred while processing your request.";
                response.Success = false;
            }
            return response;
        }

        public async Task<Core.Response.Response<bool>> AssignQuestionsAsync(AssignQuestionsDto request)
        {
            try
            {
                Core.Response.Response<bool> response = new Core.Response.Response<bool>();
                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("interview-user-detail");
                if (authDetail == null)
                {
                    response.Message = "Configurations are not found.";
                    response.Success = false;
                    return response;
                }

                var resumePaths = await _interviewRepository.GetResumesPathsByGroupIdAsync(request.GroupId);

                if (resumePaths == null || resumePaths.Count == 0)
                {
                    response.Message = $"No resumes found for the given Group ID {request.GroupId}.";
                    response.Success = false;
                    return response;
                }
                request.UserList = resumePaths!;

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.URL)
                };
               
                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);
                Core.Response.Response clientResponse = await _visionetClientService.AssignUserQuestions(request);

               // var result = await _questionRepository.SaveAssignedQuestionsAsync(request);
                return new Core.Response.Response<bool> {  Success = clientResponse.Success, Message = clientResponse.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning questions");
                return new Core.Response.Response<bool> { Data = false, Success = false, Message = "An error occurred" };
            }
        }

        public async Task<Core.Response.Response<bool>> CreateInterviewGroupUserQuestionAsync(List<CreateInterviewGroupUserQuestionDto> requests)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {

                var newQuestions = requests.Select(request => new InterviewGroupUserQuestion
                {
                    Question = request.Question,
                    InterviewGroupUserDetailId = request.InterviewGroupUserDetailId,
                    DifficultyLevel = request.DifficultyLevel,
                    Agent = request.Agent,
                    ImpactFactor = request.ImpactFactor,
                    Category = request.Category,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                }).ToList();

                await _interviewRepository.CreateInterviewGroupUserQuestionsAsync(newQuestions);
                response.Data = true;
                response.Message = StatusAndMessagesKeys.CreateSuccess;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating InterviewGroupUserQuestion.");
                response.Message = "An error occurred while processing the request.";
                response.Success = false;
            }
            return response;
        }
    }
}
