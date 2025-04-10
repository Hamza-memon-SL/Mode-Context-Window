using AutoMapper;
using Azure;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Interfaces.IService.IVisionetClientService;
using GenAiPoc.Core.Request.Jira;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using GenAiPoc.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.IdentityGovernance;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Graph.Constants;

namespace GenAiPoc.Application.Services
{
    public class BacklogBuddyService : IBacklogBuddyService
    {
        private readonly IBacklogBuddyRepository _backlogBuddyRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<BacklogBuddyService> _logger;
        private readonly IMapper _mapper;
        private IVisionetClientService _visionetClientService;

        public BacklogBuddyService(IBacklogBuddyRepository backlogBuddyRepository, IBlobStorageService blobStorageService, ILogger<BacklogBuddyService> logger, IMapper mapper, IVisionetClientService visionetClientService)
        {
            _backlogBuddyRepository = backlogBuddyRepository;
            _blobStorageService = blobStorageService;
            this._logger = logger;
            _mapper = mapper;
            this._visionetClientService = visionetClientService;
        }

        public async Task<GetAllDevopsProjectsResponse> GetAllDevopsProjects(GetAllDevopsProjectsDTO request)
        {
            try
            {
                var validateUrl = await _backlogBuddyRepository.GetRepositoryInfoFromUrl(request.url);
                if (!string.IsNullOrEmpty(validateUrl))
                {
                    var organization = validateUrl;
                    var response = await _backlogBuddyRepository.GetAllDevopsProjectsAsync(request, organization);
                    if (response.isSuccess)
                    {
                        request.url = Regex.Replace(request.url, @"\/+$", "");
                        var userAuthentication = await _backlogBuddyRepository.AddDevopsUserAuthentication(request);
                        return new GetAllDevopsProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response.Item2);
                    }
                    else
                    {
                        return new GetAllDevopsProjectsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage, null);
                    }
                }
                else
                {
                    return new GetAllDevopsProjectsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.InvalidUrlDevopsOrganization, null);
                }
            }
            catch (Exception ex)
            {
                return new GetAllDevopsProjectsResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, null);
            }
        }
        public async Task<ImportAllDevopsUserStoriesResponse> ImportAllDevopsUserStories(ImportAllDevopsUserStoriesDTO request)
        {
            try
            {
                var mappingRequest = _mapper.Map<UserAuthenticationDTO>(request);
                var user = await _backlogBuddyRepository.GetUserAuthentication(mappingRequest);
                if (user != null)
                {
                    var checkBacklogDevopsProject = await _backlogBuddyRepository.CheckIfBacklogBuddyProjectExist(request);
                    var checkBacklogDevopsUserStoriesFromDB = await _backlogBuddyRepository.CheckIfBacklogBuddyDevopsUserStoryExist(request);

                    if (checkBacklogDevopsProject != null && checkBacklogDevopsUserStoriesFromDB == null)
                    {
                        var response = await _backlogBuddyRepository.GetUserStoryFromDevopsAsync(request, user);
                        if (response.isSuccess && response.Item2.count > 0)
                        {
                            var userStories = await _backlogBuddyRepository.AddDevopsUserStoriesInDb(request, response.Item2, user);
                            return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                        }
                        else if (response.isSuccess && response.Item2.count == 0)
                        {
                            return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                        }
                        else
                        {
                            return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessageForPatExpire);
                        }
                    }
                    else if (checkBacklogDevopsProject == null)
                    {
                        var addBacklogProject = await _backlogBuddyRepository.AddBackLogBuddyProject(request);
                        if (addBacklogProject)
                        {
                            var response = await _backlogBuddyRepository.GetUserStoryFromDevopsAsync(request, user);
                            if (response.isSuccess && response.Item2.count > 0)
                            {
                                var userStories = await _backlogBuddyRepository.AddDevopsUserStoriesInDb(request, response.Item2, user);
                                return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                            }
                            else if (response.isSuccess && response.Item2.count == 0)
                            {
                                return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                            }
                            else
                            {
                                return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessageForPatExpire);
                            }
                        }
                        return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAddedFailed);
                    }
                    return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.BackLogBuddyProjectAlreadyExist);
                }
                else
                {
                    return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
            }
        }

        public async Task<GetAllJiraProjectsResponse> GetAllJiraProjects(GetAllJiraProjectsDTO request)
        {
            try
            {
                var response = await _backlogBuddyRepository.GetAllJiraProjectsAsync(request);
                if (response.isSuccess)
                {
                    request.url = Regex.Replace(request.url, @"\/+$", "");
                    var userAuthentication = await _backlogBuddyRepository.AddJiraUserAuthentication(request);
                    return new GetAllJiraProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response.Item2);
                }
                else
                {
                    return new GetAllJiraProjectsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage, null);
                }
            }
            catch (Exception ex)
            {
                return new GetAllJiraProjectsResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, null);
            }
        }

        //public async Task<ImportAllJiraUserStoriesResponse> ImportAllJiraUserStories(ImportAllJiraUserStoriesDTO request)
        //{
        //    try
        //    {
        //        var mappingRequest = _mapper.Map<ImportAllDevopsUserStoriesDTO>(request);
        //        var authenticationRequest=_mapper.Map<UserAuthenticationDTO>(request);
        //        var user = await _backlogBuddyRepository.GetUserAuthentication(authenticationRequest);
        //        if (user != null)
        //        {
        //            var checkBacklogJiraProject = await _backlogBuddyRepository.CheckIfBacklogBuddyProjectExist(mappingRequest);
        //            if (checkBacklogJiraProject == null)
        //            {
        //                var addBacklogProject = await _backlogBuddyRepository.AddBackLogBuddyProject(mappingRequest);
        //                if (addBacklogProject)
        //                {
        //                    var response = await _backlogBuddyRepository.GetUserStoryFromJiraAsync(request, user);
        //                    if (response.isSuccess && response.Item2.Total > 0)
        //                    {
        //                        var userStories = await _backlogBuddyRepository.AddJiraUserStoriesInDb(request, response.Item2, user);
        //                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
        //                    }
        //                    else if (response.isSuccess && response.Item2.Total == 0)
        //                    {
        //                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
        //                    }
        //                    else
        //                    {
        //                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
        //                    }
        //                }
        //                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.BackLogBuddyProjectAlreadyExist);
        //            }
        //            return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAddedFailed);
        //        }
        //        else
        //        {
        //            return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
        //    }
        //}

        public async Task<ImportAllJiraUserStoriesResponse> ImportAllJiraUserStories(ImportAllJiraUserStoriesDTO request)
        {
            try
            {
                var mappingRequest = _mapper.Map<ImportAllDevopsUserStoriesDTO>(request);
                var authenticationRequest = _mapper.Map<UserAuthenticationDTO>(request);
                var user = await _backlogBuddyRepository.GetUserAuthentication(authenticationRequest);
                if (user != null)
                {
                    var checkBacklogJiraProject = await _backlogBuddyRepository.CheckIfBacklogBuddyProjectExist(mappingRequest);
                    var checkBacklogJiraUserStoriesFromDB = await _backlogBuddyRepository.CheckIfBacklogBuddyJiraUserStoryExist(mappingRequest);

                    if (checkBacklogJiraProject != null && checkBacklogJiraUserStoriesFromDB == null)
                    {
                        var response = await _backlogBuddyRepository.GetUserStoryFromJiraAsync(request, user);
                        if (response.isSuccess && response.Item2.Total > 0)
                        {
                            var userStories = await _backlogBuddyRepository.AddJiraUserStoriesInDb(request, response.Item2, user);
                            return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                        }
                        else if (response.isSuccess && response.Item2.Total == 0)
                        {
                            return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                        }
                        else
                        {
                            return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessageForPatExpire);
                        }
                    }
                    else if (checkBacklogJiraProject == null)
                    {
                        var addBacklogProject = await _backlogBuddyRepository.AddBackLogBuddyProject(mappingRequest);
                        if (addBacklogProject)
                        {
                            var response = await _backlogBuddyRepository.GetUserStoryFromJiraAsync(request, user);
                            if (response.isSuccess && response.Item2.Total > 0)
                            {
                                var userStories = await _backlogBuddyRepository.AddJiraUserStoriesInDb(request, response.Item2, user);
                                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                            }
                            else if (response.isSuccess && response.Item2.Total == 0)
                            {
                                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                            }
                            else
                            {
                                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessageForPatExpire);
                            }
                        }
                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAddedFailed);

                    }
                    return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.BackLogBuddyProjectAlreadyExist);
                }
                else
                {
                    return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
            }
        }

        public async Task<GetAllBacklogBuddyProjectsResponse> GetAllBacklogBuddyProjects(string authToken)
        {
            try
            {
                var projectResponses = await _backlogBuddyRepository.GetAllBacklogBuddyProjectsAsync(authToken);
                if (projectResponses != null && projectResponses.Count > 0)
                {
                    return new GetAllBacklogBuddyProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, projectResponses);
                }
                else
                {
                    return new GetAllBacklogBuddyProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, new List<ProjectResponse>());
                }
            }
            catch (Exception ex)
            {
                return new GetAllBacklogBuddyProjectsResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, new List<ProjectResponse>());
            }

        }
        public async Task<GetAllBacklogBuddyUserStoriesResponse> GetAllBacklogBuddyUserStories(string authToken, string projectId, string projectType)
        {
            try
            {
                if (projectType == "Devops")
                {
                    var response = await _backlogBuddyRepository.GetAllBacklogBuddyDevopsUserStoriesAsync(authToken, projectId);
                    if (response.Count > 0)
                    {
                        return new GetAllBacklogBuddyUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response, null);
                    }
                }
                else if (projectType == "Jira")
                {
                    var response = await _backlogBuddyRepository.GetAllBacklogBuddyJiraUserStoriesAsync(authToken, projectId);
                    if (response.Count > 0)
                    {
                        return new GetAllBacklogBuddyUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, null, response);
                    }
                }
                return new GetAllBacklogBuddyUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null, null);
            }
            catch (Exception ex)
            {
                return new GetAllBacklogBuddyUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, null, null);
            }

        }

        public async Task<ImportAllDevopsUserStoriesResponse> SyncAllDevopsUserStories(SyncAllDevopsUserStoriesDTO request)
        {
            try
            {
                var mappingRequest = _mapper.Map<UserAuthenticationDTO>(request);
                var user = await _backlogBuddyRepository.GetUserAuthentication(mappingRequest);
                if (user != null)
                {

                    var response = await _backlogBuddyRepository.GetUserStoryFromDevopsAsync(request, user);
                    if (response.isSuccess && response.Item2.count > 0)
                    {
                        var userStories = await _backlogBuddyRepository.SyncDevopsUserStories(request, response.Item2, user);
                        return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                    }
                    else if (response.isSuccess && response.Item2.count == 0)
                    {
                        return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                    }
                    else
                    {
                        return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                    }

                }
                else
                {
                    return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new ImportAllDevopsUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
            }
        }
        public async Task<ImportAllJiraUserStoriesResponse> SyncAllJiraUserStories(SyncAllJiraUserStoriesDTO request)
        {
            try
            {
                //var mappingRequest = _mapper.Map<UserAuthenticationDTO>(request);
                var mappingRequest = _mapper.Map<UserAuthenticationDTO>(request);
                var user = await _backlogBuddyRepository.GetUserAuthentication(mappingRequest);
                if (user != null)
                {

                    var response = await _backlogBuddyRepository.GetUserStoryFromJiraAsync(request, user);
                    if (response.isSuccess && response.Item2.Total > 0)
                    {

                        var userStories = await _backlogBuddyRepository.SyncJiraUserStories(request, response.Item2, user);
                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                    }
                    else if (response.isSuccess && response.Item2.Total == 0)
                    {
                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoStoriesFound);
                    }
                    else
                    {
                        return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                    }

                }
                else
                {
                    return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new ImportAllJiraUserStoriesResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
            }
        }

        public async Task<GetHealthAnalyticsResponse> GetProjectHealth(string authToken, string projectId, string projectType)
        {
            try
            {
                if (projectType == "Devops")
                {
                    var response = await _backlogBuddyRepository.GetBacklogBuddyDevopsProjectHealth(authToken, projectId);
                    if (response != null)
                    {
                        return new GetHealthAnalyticsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                    }
                }
                else if (projectType == "Jira")
                {
                    var response = await _backlogBuddyRepository.GetBacklogBuddyJiraProjectHealth(authToken, projectId);
                    if (response != null)
                    {
                        return new GetHealthAnalyticsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                    }
                }

                return new GetHealthAnalyticsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
            }
            catch (Exception ex)
            {
                return new GetHealthAnalyticsResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, null);
            }
        }
        public async Task<UserStoryLevelHealthResponse> GetUserStoryLevelHealth(string authToken, string projectId, string projectType, int pageNumber, int pageSize)
        {
            try
            {
                if (projectType.Equals("devops", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await _backlogBuddyRepository.GetBacklogBuddyDevopsUserStoryHealth(authToken, projectId, pageNumber, pageSize);
                    if (response != null)
                    {
                        return new UserStoryLevelHealthResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                    }
                }
                else if (projectType.Equals("jira", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await _backlogBuddyRepository.GetBacklogBuddyJiraUserStoryHealth(authToken, projectId, pageNumber, pageSize);
                    if (response != null)
                    {
                        return new UserStoryLevelHealthResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                    }
                }

                return new UserStoryLevelHealthResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
            }
            catch (Exception ex)
            {
                return new UserStoryLevelHealthResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message, null);
            }
        }

        public async Task<Core.Response.Response<JiraAnalyticsResponse>> GetJiraSementicResults(JiraSementicRequest request)
        {
            var responses = new Core.Response.Response<JiraAnalyticsResponse>();
            try
            {
                // Log start of operation
                _logger.LogInformation("Starting GetJiraSementicResults for request: {@Request}", request);

                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("jmfamily");
                if (authDetail == null)
                {
                    _logger.LogWarning("Authentication details not found for key: jmfamily");
                    responses.Message = "Authentication details not found.";
                    responses.Success = false;
                    return responses;
                }

                // Initialize HttpClient with BaseAddress from auth details
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.JiraBaseUrl) // Pass dynamic URL from the database here
                };

                // Encode credentials as Base64 and add to headers
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authDetail.username}:{authDetail.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);

                _logger.LogInformation("Calling Visionet API to fetch Jira semantic results...");
                JiraAnalyticsResponse jiraResponse = await _visionetClientService.GetJiraSementicResultsAsync(request.Request);


                _logger.LogInformation("Adding or updating Jira analytics data in the database...");
                await _backlogBuddyRepository.AddOrUpdateJiraAnalyticsAsync(jiraResponse, request);

                responses.Data = jiraResponse;
                responses.Success = true;
                responses.Message = "Operation completed successfully.";
                _logger.LogInformation("GetJiraSementicResults completed successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while calling Visionet API.");
                responses.Message = "Failed to connect to the Visionet API.";
                responses.Success = false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while adding or updating Jira analytics.");
                responses.Message = "Failed to update the database.";
                responses.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in JiraSementicResults.");
                responses.Message = "An unexpected error occurred.";
                responses.Success = false;
            }

            return responses;
        }

        public async Task<Core.Response.Response<dynamic>> GetJiraAnalyticsSummaryAsync(string authToken)
        {
            Core.Response.Response<dynamic> response = null;
            try
            {
                var summary = await _backlogBuddyRepository.GetAnalyticsSummaryAsync(authToken);

                return new Core.Response.Response<dynamic>
                {
                    Data = summary,
                    Success = true,
                    Message = "Operation completed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching analytics summary at {Time}", DateTime.UtcNow);

                return new Core.Response.Response<dynamic>
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<Core.Response.Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryAsync(JiraAnalyticsHistoryRequest request)
        {

            Core.Response.Response<JiraSearchAnalyticsHistoryResponse> response = new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>();
            try
            {
                // Validate date range
                if (request.startDate > request.endDate)
                {
                    return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                    {

                        Success = false,
                        Message = $"Invalid date range: StartDate {request.startDate} is greater than EndDate {request.endDate}"
                    };
                }

                // var jiraAnalyticsHistory = await _backlogBuddyRepository.GetJiraAnalyticsHistoryWithFilterAsync(request);
                var jiraAnalyticsHistory = await _backlogBuddyRepository.GetJiraSearchAnalyticsPaginatedAsync(request);


                return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                {
                    Data = jiraAnalyticsHistory,
                    Success = true,
                    Message = "Operation completed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Jira analytics history.");

                return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                {

                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }
        public async Task<Core.Response.Response<JiraSearchAnalyticsHistoryResponse>> GetJiraAnalyticsHistoryForExtensionAsync(JiraAnalyticsHistoryRequest request)
        {

            Core.Response.Response<JiraSearchAnalyticsHistoryResponse> response = new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>();
            try
            {
                // Validate date range
                if (request.startDate > request.endDate)
                {
                    return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                    {

                        Success = false,
                        Message = $"Invalid date range: StartDate {request.startDate} is greater than EndDate {request.endDate}"
                    };
                }

                // var jiraAnalyticsHistory = await _backlogBuddyRepository.GetJiraAnalyticsHistoryWithFilterAsync(request);
                var jiraAnalyticsHistory = await _backlogBuddyRepository.GetJiraSearchAnalyticsPaginatedExtensionAsync(request);


                return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                {
                    Data = jiraAnalyticsHistory,
                    Success = true,
                    Message = "Operation completed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Jira analytics history.");

                return new Core.Response.Response<JiraSearchAnalyticsHistoryResponse>
                {

                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<Core.Response.Response<bool>> GetJiraAnalyticsTimeSavedAsync(JiraAnalyticsTimeSavedRequest request)
        {
            var response = new Core.Response.Response<bool> { Success = false };
            try
            {

                await _backlogBuddyRepository.GetJiraAnalyticsTimeSavedAsync(request);

                response.Success = true;
                response.Message = "Operation completed successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Jira analytics time saved.");
                response.Success = false;
                response.Message = "An unexpected error occurred: " + ex.Message;
                return response;
            }
        }

        public async Task<Core.Response.Response<JiraTreeRootResponse>> GetAllJiraArtifactsTreeView(bool sync)
        {
            var responses = new Core.Response.Response<JiraTreeRootResponse>();
            try
            {
                if (!sync)
                {
                    var jiraArtifact = await _backlogBuddyRepository.GetJiraArtifactsTree();

                    if (!string.IsNullOrEmpty(jiraArtifact))
                    {
                        var result = JsonConvert.DeserializeObject<JiraTreeRootResponse>(jiraArtifact);

                        return new Core.Response.Response<JiraTreeRootResponse>
                        {
                            Data = result,
                            Success = true,
                            Message = "Operation completed successfully."
                        };

                    }
                }

                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("jmfamily-tree");
                if (authDetail == null)
                {
                    _logger.LogWarning("Authentication details not found for key: jmfamily-tree");
                    responses.Message = "Authentication details not found.";
                    responses.Success = false;
                    return responses;
                }

                // Initialize HttpClient with BaseAddress from auth details
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.JiraBaseUrl)
                };

                // Encode credentials as Base64 and add to headers
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authDetail.username}:{authDetail.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);

                _logger.LogInformation("Calling Visionet API to fetch Jira tree results...");
                JiraTreeRootResponse jiraResponse = await _visionetClientService.GetAllJiraArtifactsTreeView();


                JiraSync jiraSync = new JiraSync
                {
                    JsonMapping = JsonConvert.SerializeObject(jiraResponse),
                    CreatedDate = DateTime.UtcNow,
                    key = "jmfamily-tree"
                };

                await _backlogBuddyRepository.SyncJiraArtifacts(jiraSync);

                responses.Data = jiraResponse;
                responses.Success = true;
                responses.Message = "Operation completed successfully.";
                _logger.LogInformation("GetAllJiraArtifactsTreeView completed successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while calling Visionet API.");
                responses.Message = "Failed to connect to the Visionet API.";
                responses.Success = false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while adding or updating Jira analytics.");
                responses.Message = "Failed to update the database.";
                responses.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in JiraSementicResults.");
                responses.Message = "An unexpected error occurred.";
                responses.Success = false;
            }

            return responses;
        }

        public async Task<Core.Response.Response<CreateJiraStoryResponse>> CreateJiraStoryAsync(CreateJiraStoryRequest request)
        {
            var responses = new Core.Response.Response<CreateJiraStoryResponse>();
            try
            {

                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("jmfamily-create-user-story");
                if (authDetail == null)
                {
                    _logger.LogWarning("Authentication details not found for key: jmfamily-create-user-story");
                    responses.Message = "Authentication details not found.";
                    responses.Success = false;
                    return responses;
                }


                request.fields.issuetype = new UserStoryIssuetype { name = "Story" };


                // Initialize HttpClient with BaseAddress from auth details
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.JiraBaseUrl)
                };

                // Encode credentials as Base64 and add to headers
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authDetail.username}:{authDetail.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);

                _logger.LogInformation("Calling Visionet API to fetch Jira tree results...");
                CreateJiraStoryResponse jiraResponse = await _visionetClientService.CreateJiraUserStory(request);

                responses.Data = jiraResponse;
                responses.Success = true;
                responses.Message = "Operation completed successfully.";
                _logger.LogInformation("GetAllJiraArtifactsTreeView completed successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while calling Visionet API.");
                responses.Message = "Failed to connect to the Visionet API.";
                responses.Success = false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while adding or updating Jira analytics.");
                responses.Message = "Failed to update the database.";
                responses.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in JiraSementicResults.");
                responses.Message = "An unexpected error occurred.";
                responses.Success = false;
            }

            return responses;
        }

        public async Task<Core.Response.Response<CreateJiraTestCaseResponse>> CreateJiraTestCaseAsync(CreateJiraTestCaseRequest request)
        {
            var responses = new Core.Response.Response<CreateJiraTestCaseResponse>();
            try
            {

                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("jmfamily-create-user-story");
                if (authDetail == null)
                {
                    _logger.LogWarning("Authentication details not found for key: jmfamily-create-user-story");
                    responses.Message = "Authentication details not found.";
                    responses.Success = false;
                    return responses;
                }

                // Initialize HttpClient with BaseAddress from auth details
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.JiraBaseUrl)
                };

                request.fields.issuetype = new JiraTestCaseIssuetype { name = "Test" };


                // Encode credentials as Base64 and add to headers
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authDetail.username}:{authDetail.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);

                _logger.LogInformation("Calling Visionet API to fetch Jira tree results...");
                CreateJiraTestCaseResponse jiraResponse = await _visionetClientService.CreateJiraTestCase(request);

                responses.Data = jiraResponse;
                responses.Success = true;
                responses.Message = "Operation completed successfully.";
                _logger.LogInformation("GetAllJiraArtifactsTreeView completed successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while calling Visionet API.");
                responses.Message = "Failed to connect to the Visionet API.";
                responses.Success = false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while adding or updating Jira analytics.");
                responses.Message = "Failed to update the database.";
                responses.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in JiraSementicResults.");
                responses.Message = "An unexpected error occurred.";
                responses.Success = false;
            }

            return responses;
        }

        public async Task<Core.Response.Response<GenerateJiraArtifactResponse>> GenerateJiraArtifactAsync(GenerateJiraArtifactRequest request)
        {
            var responses = new Core.Response.Response<GenerateJiraArtifactResponse>();
            try
            {

                // Fetch authentication details
                var authDetail = await _backlogBuddyRepository.GetUserAuthenticationDetailsByKey("jmfamily-generate-artifact");
                if (authDetail == null)
                {
                    _logger.LogWarning("Authentication details not found for key: jmfamily-generate-artifact");
                    responses.Message = "Authentication details not found.";
                    responses.Success = false;
                    return responses;
                }

                // Initialize HttpClient with BaseAddress from auth details
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(authDetail.JiraBaseUrl)
                };

                // Encode credentials as Base64 and add to headers
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authDetail.username}:{authDetail.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                _visionetClientService = RestService.For<IVisionetClientService>(httpClient);

                _logger.LogInformation("Calling Visionet API to fetch Generate Artifact results...");
                GenerateJiraArtifactResponse jiraResponse = await _visionetClientService.GenerateJiraArtifact(request);

                responses.Data = jiraResponse;
                responses.Success = true;
                responses.Message = "Operation completed successfully.";
                _logger.LogInformation("GetAllJiraArtifactsTreeView completed successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while calling Visionet API.");
                responses.Message = "Failed to connect to the Visionet API.";
                responses.Success = false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while adding or updating Jira analytics.");
                responses.Message = "Failed to update the database.";
                responses.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in JiraSementicResults.");
                responses.Message = "An unexpected error occurred.";
                responses.Success = false;
            }

            return responses;
        }

        public async Task<Core.Response.Response<bool>> DeleteBacklogBuddyProjectsAsync(string authToken, string projectId)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(projectId))
                {
                    _logger.LogWarning("Invalid input parameters: authToken or projectId is null or empty.");
                    response.Message = "Invalid input parameters.";
                    response.Success = false;

                    return response;
                }

                _logger.LogInformation($"Starting deletion process for AuthToken: {authToken} and ProjectId: {projectId}");

                var projectsToDelete = await _backlogBuddyRepository.DeleteBacklogBuddyProjectsAsync(authToken, projectId);

                if (!projectsToDelete)
                {
                    _logger.LogWarning($"No projects found for deletion with AuthToken: {authToken} and ProjectId: {projectId}");
                    response.Message = $"No projects found for deletion with ProjectId: {projectId}";
                    response.Success = false;

                    return response;
                }
                response.Data = true;
                response.Message = $"Operation completed successfully.";
                response.Success = true;

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting BacklogBuddyProjects.");
                response.Message = $"An error occurred while deleting BacklogBuddyProjects.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Core.Response.Response<ProjectCrudAnalysis>> GetProjectsWithCrudAnalysis(string authToken, string projectId)
        {
            var devopsUserStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);
            //var projectSummaries = new List<ProjectCrudAnalysis>();

            //var groupedProjects = projects.GroupBy(project => project.DevopsProjectId).ToList();


            var summary = new ProjectCrudAnalysis
            {
                //ProjectId = projectSummary.Key,
                CrudSummary = AnalyzeCrudOperations(devopsUserStories)
            };

            //foreach (var userStory in devopsUserStories)
            //{
            //    var summary = new ProjectCrudAnalysis
            //    {
            //        //ProjectId = projectSummary.Key,
            //        CrudSummary = AnalyzeCrudOperations(projectSummary.ToList())
            //    };
            //    projectSummaries.Add(summary);
            //}

            return new Core.Response.Response<ProjectCrudAnalysis>
            {
                Data = summary,
                Message = StatusAndMessagesKeys.SuccessMessage,
                Success = true
            };
        }

        public async Task<ResponseList<CrudAnalysis>> GetUserLevelCrudAnalysis(string authToken, string projectId)
        {
            try
            {
                ResponseList<CrudAnalysis> response = new ResponseList<CrudAnalysis>();
                List<CrudAnalysisJson> crudAnalysisJsons = new List<CrudAnalysisJson>();
                List<CrudAnalysis> crudAnalysis = new List<CrudAnalysis>();
                var userStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);

                if (userStories == null)
                {
                    response.Success = false;
                    response.Message = StatusAndMessagesKeys.GetAllFailed;
                    return response;
                }

                foreach (var userStory in userStories)
                {
                    try
                    {
                        var crudAnalysisTemp = JsonConvert.DeserializeObject<CrudAnalysisJson>(userStory.CrudAnalysisJson);
                        //crudAnalysisJsons.Add(crudAnalysisTemp);

                        crudAnalysis.Add(new CrudAnalysis
                        {
                            CrudAnalysisJson = crudAnalysisTemp,
                            ProjectId = userStory.DevopsProjectId,
                            Title = userStory.Title,
                            UserStoryId = userStory.Id
                        });
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                response.Success = true;
                response.Data = crudAnalysis;
                response.Message = StatusAndMessagesKeys.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseList<FunctionalSize>> GetUserLevelFunctionalSizeAnalysis(string authToken, string projectId)
        {
            try
            {
                ResponseList<FunctionalSize> response = new ResponseList<FunctionalSize>();
                List<FunctionalSize> functionalSizes = new List<FunctionalSize>();

                var userStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);

                if (userStories == null)
                {
                    response.Success = false;
                    response.Message = StatusAndMessagesKeys.GetAllFailed;
                    return response;
                }

                foreach (var userStory in userStories)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(userStory.FunctionalSizeJson))
                        {
                            var functionalSizeJson = JsonConvert.DeserializeObject<FunctionalSizeJson>(userStory.FunctionalSizeJson);
                            functionalSizes.Add(new FunctionalSize
                            {
                                FunctionalSizeJson = functionalSizeJson,
                                ProjectId = userStory.DevopsProjectId,
                                Title = userStory.Title,
                                UserStoryId = userStory.Id
                            });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                response.Success = true;
                response.Data = functionalSizes;
                response.Message = StatusAndMessagesKeys.SuccessMessage;

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponseList<BacklogAcceptanceCriterianResponse>> GetUserLevelAcceptanceCriterianAnalysis(string authToken, string projectId)
        {
            try
            {
                ResponseList<BacklogAcceptanceCriterianResponse> response = new ResponseList<BacklogAcceptanceCriterianResponse>();
                List<BacklogAcceptanceCriterianResponse> acceptanceCriteriasResponse = new List<BacklogAcceptanceCriterianResponse>();

                var userStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);

                if (userStories == null)
                {
                    response.Success = false;
                    response.Message = StatusAndMessagesKeys.GetAllFailed;
                    return response;
                }

                foreach (var userStory in userStories)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(userStory.AcceptanceCriteriaJson))
                        {
                            var accCriteriaJson = JsonConvert.DeserializeObject<AcceptanceCriterian>(userStory.AcceptanceCriteriaJson);

                            acceptanceCriteriasResponse.Add(new BacklogAcceptanceCriterianResponse
                            {
                                AcceptanceCriteriaJson = accCriteriaJson,
                                ProjectId = userStory.DevopsProjectId,
                                Title = userStory.Title,
                                UserStoryId = userStory.Id
                            });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                response.Success = true;
                response.Data = acceptanceCriteriasResponse;
                response.Message = StatusAndMessagesKeys.SuccessMessage;

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Core.Response.Response<FunctionalSizeProjectResponse>> GetProjectsWithFunctionalSizeAnalysis(string authToken, string projectId)
        {
            var devopsUserStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);

            var response = AnalyzeFunctionalOperations(devopsUserStories);

            if (response == null)
            {
                return new Core.Response.Response<FunctionalSizeProjectResponse>
                {
                    Message = StatusAndMessagesKeys.GetAllFailed,
                    Success = false
                };
            }

            return new Core.Response.Response<FunctionalSizeProjectResponse>
            {
                Data = response,
                Message = StatusAndMessagesKeys.SuccessMessage,
                Success = true
            };
        }

        public async Task<Core.Response.Response<ProjectLevelAcceptanceCriteriaResponse>> GetProjectsWithAcceptanceCriteriaAnalysis(string authToken, string projectId)
        {
            var devopsUserStories = await _backlogBuddyRepository.GetDevOpsUserStoriesByProject(authToken, projectId);

            var response = AnalyzeAcceptanceCriteria(devopsUserStories);

            if (response == null)
            {
                return new Core.Response.Response<ProjectLevelAcceptanceCriteriaResponse>
                {
                    Message = StatusAndMessagesKeys.GetAllFailed,
                    Success = false
                };
            }

            return new Core.Response.Response<ProjectLevelAcceptanceCriteriaResponse>
            {
                Data = response,
                Message = StatusAndMessagesKeys.SuccessMessage,
                Success = true
            };
        }


        private CrudAnalysisSummary AnalyzeCrudOperations(List<DevopsUserStories> userStories)
        {
            int totalEntities = 0;
            int totalCrudOperations = 0;
            int createCount = 0;
            int readCount = 0;
            int updateCount = 0;
            int deleteCount = 0;
            List<CrudBarChart> crudBarCharts = new List<CrudBarChart>();

            foreach (var story in userStories)
            {
                try
                {

                    if (string.IsNullOrEmpty(story.CrudAnalysisJson)) continue;

                    var crudAnalysis = JsonConvert.DeserializeObject<CrudAnalysisJson>(story.CrudAnalysisJson);

                    if (crudAnalysis == null || crudAnalysis.Entities == null) continue;

                    totalEntities += crudAnalysis.Entities.Count;


                    createCount = 0;
                    readCount = 0;
                    updateCount = 0;
                    deleteCount = 0;
                    foreach (var entity in crudAnalysis.Entities)
                    {

                        totalCrudOperations += entity.Operations.Count;
                        foreach (var operation in entity.Operations)
                        {
                            if (!string.IsNullOrEmpty(operation.Read))
                            {
                                readCount++;
                            }
                            if (!string.IsNullOrEmpty(operation.Create))
                            {
                                createCount++;
                            }
                            if (!string.IsNullOrEmpty(operation.Update))
                            {
                                updateCount++;
                            }
                            if (!string.IsNullOrEmpty(operation.Delete))
                            {
                                deleteCount++;
                            }

                            //switch (operation.Operation.ToLower())
                            //{
                            //    case "create": createCount++; break;
                            //    case "read": readCount++; break;
                            //    case "update": updateCount++; break;
                            //    case "delete": deleteCount++; break;
                            //}
                        }
                    }

                    crudBarCharts.Add(new CrudBarChart
                    {
                        Title = story.Title,
                        CreateCount = createCount,
                        DeleteCount = deleteCount,
                        ReadCount = readCount,
                        UpdateCount = updateCount,
                        EntitiesCount = crudAnalysis.Entities.Count,
                    });

                }
                catch (Exception)
                {
                    continue;
                }

            }

            var groupedCrudBarCharts = crudBarCharts
                .GroupBy(x => x.Title)
                .Select(g => new CrudBarChart
                {
                    Title = g.Key,
                    CreateCount = g.Sum(x => x.CreateCount),
                    ReadCount = g.Sum(x => x.ReadCount),
                    UpdateCount = g.Sum(x => x.UpdateCount),
                    DeleteCount = g.Sum(x => x.DeleteCount)
                })
                .ToList();

            var top5ComplexStories = groupedCrudBarCharts
                .OrderByDescending(s => s.TotalCrudCount) // Sort by combined (Create + Update + Delete)
                .Take(5)
                .ToList();

            var pieChartData = new Dictionary<string, double>();
            if (totalCrudOperations > 0)
            {
                pieChartData["Create"] = (crudBarCharts.Select(x => x.CreateCount).Sum() / (double)totalCrudOperations) * 100;
                pieChartData["Read"] = (crudBarCharts.Select(x => x.ReadCount).Sum() / (double)totalCrudOperations) * 100;
                pieChartData["Update"] = (crudBarCharts.Select(x => x.UpdateCount).Sum() / (double)totalCrudOperations) * 100;
                pieChartData["Delete"] = (crudBarCharts.Select(x => x.DeleteCount).Sum() / (double)totalCrudOperations) * 100;
            }

            return new CrudAnalysisSummary
            {
                TotalEntities = totalEntities,
                TotalCrudOperations = totalCrudOperations,
                CrudBarChart = crudBarCharts,
                CRUDReportsPieChart = pieChartData,
                Top5ComplexStories = top5ComplexStories
            };
        }

        private FunctionalSizeProjectResponse AnalyzeFunctionalOperations(List<DevopsUserStories> userStories)
        {
            double totalInputsPercentageForBarChart = 0;
            double totalOutputsPercentageForBarChart = 0;
            double totalQueriesPercentageForBarChart = 0;
            double totalFilesPercentageForBarChart = 0;
            double totalInterfacesPercentageForBarChart = 0;
            List<FunctionalSizeBarChart> functionalSizeBarCharts = new List<FunctionalSizeBarChart>();
            foreach (var userStory in userStories)
            {
                if (userStory != null && !string.IsNullOrEmpty(userStory.FunctionalSizeJson))
                {
                    var functionalSizeJson = JsonConvert.DeserializeObject<FunctionalSizeJson>(userStory.FunctionalSizeJson);

                    if (string.IsNullOrEmpty(functionalSizeJson.error))
                    {
                        totalInputsPercentageForBarChart += functionalSizeJson.Inputs.Count();
                        totalOutputsPercentageForBarChart += functionalSizeJson.Outputs.Count();
                        totalQueriesPercentageForBarChart += functionalSizeJson.Queries.Count();
                        totalFilesPercentageForBarChart += functionalSizeJson.Files.Count();
                        totalInterfacesPercentageForBarChart += functionalSizeJson.Interfaces.Count();


                        //BarChart Implementation
                        FunctionalSizeBarChart functionalSizeBarChart = new FunctionalSizeBarChart()
                        {
                            UserStoryId = userStory.Id,
                            Title = userStory.Title,
                            InputCount = functionalSizeJson.Inputs.Count(),
                            OutputCount = functionalSizeJson.Outputs.Count(),
                            FileCount = functionalSizeJson.Files.Count(),
                            InterfaceCount = functionalSizeJson.Interfaces.Count(),
                            QueryCount = functionalSizeJson.Queries.Count()
                        };

                        functionalSizeBarCharts.Add(functionalSizeBarChart);

                    }
                }
            }
            double total = totalInputsPercentageForBarChart +
                totalOutputsPercentageForBarChart + totalQueriesPercentageForBarChart
                + totalFilesPercentageForBarChart + totalInterfacesPercentageForBarChart;

            if (total > 0)
            {

                FunctionalSizePieChart functionalSizePieChart = new FunctionalSizePieChart()
                {
                    InputsPercentage = (totalInputsPercentageForBarChart / total) * 100,
                    FilesPercentage = (totalFilesPercentageForBarChart / total) * 100,
                    InterfacesPercentage = (totalInterfacesPercentageForBarChart / total) * 100,
                    OutputsPercentage = (totalOutputsPercentageForBarChart / total) * 100,
                    QueriesPercentage = (totalQueriesPercentageForBarChart / total) * 100
                };


                // Top 5 Complex implementation

                var top5FunctionalSizeBarCharts = functionalSizeBarCharts
                            .OrderByDescending(f => f.InputCount + f.OutputCount + f.QueryCount + f.FileCount + f.InterfaceCount)
                            .Take(5)
                            .ToList();


                return new FunctionalSizeProjectResponse
                {
                    FunctionalSizePieChart = functionalSizePieChart,
                    FunctionalSizeBarChart = functionalSizeBarCharts,
                    Top5ComplexFunctions = top5FunctionalSizeBarCharts
                };
            }

            return null;
        }


        private ProjectLevelAcceptanceCriteriaResponse AnalyzeAcceptanceCriteria(List<DevopsUserStories> userStories)
        {
            ProjectLevelAcceptanceCriteriaResponse projectLevelAcceptanceCriteriaResponse = new ProjectLevelAcceptanceCriteriaResponse();

            try
            {
                int userStoriesCount = userStories != null ? userStories.Count() : 0;
                //List<AcceptanceCriterian> acceptanceCriterians = new List<AcceptanceCriterian>();
                List<BacklogAcceptanceCriterianResponse> backlogAcceptanceCriterianResponses = new List<BacklogAcceptanceCriterianResponse>();

                double GetAverageClarity()
                {
                    return backlogAcceptanceCriterianResponses.SelectMany(x => x.AcceptanceCriteriaJson.AcceptanceCriteria)?.Any() == true
                        ? backlogAcceptanceCriterianResponses.Select(x => x.AcceptanceCriteriaJson)
                            .Where(x => x.AcceptanceCriteria != null)
                            .SelectMany(x => x.AcceptanceCriteria)
                            .Average(c => c.Clarity)
                        : 0;
                }

                (double Low, double Medium, double High) GetOverallClarityRatios()
                {
                    var allCriteria = backlogAcceptanceCriterianResponses.Select(x => x.AcceptanceCriteriaJson)?
                        .Where(x => x.AcceptanceCriteria != null)
                        .SelectMany(x => x.AcceptanceCriteria)
                        .ToList();

                    if (allCriteria == null || !allCriteria.Any())
                        return (0, 0, 0);

                    int total = allCriteria.Count;

                    double low = (allCriteria.Count(c => c.Clarity < 3) / (double)total) * 100;
                    double medium = (allCriteria.Count(c => c.Clarity == 4) / (double)total) * 100;
                    double high = (allCriteria.Count(c => c.Clarity == 5) / (double)total) * 100;

                    return (low, medium, high);
                }

                foreach (var userStory in userStories)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(userStory.AcceptanceCriteriaJson))
                        {
                            var accCriteriaJson = JsonConvert.DeserializeObject<AcceptanceCriterian>(userStory.AcceptanceCriteriaJson);

                            if (accCriteriaJson != null)
                            {
                                if (string.IsNullOrEmpty(accCriteriaJson.error))
                                {
                                    //acceptanceCriterians.Add(accCriteriaJson);

                                    backlogAcceptanceCriterianResponses.Add(new BacklogAcceptanceCriterianResponse
                                    {
                                        AcceptanceCriteriaJson = accCriteriaJson,
                                        ProjectId = userStory.DevopsProjectId,
                                        Title = userStory.Title,
                                        UserStoryId = userStory.Id,

                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                int totalAcceptanceCriteria = backlogAcceptanceCriterianResponses
                    .Select(x => x.AcceptanceCriteriaJson)
                    .Sum(x => x.AcceptanceCriteria.Count());
                //.Select(x => x.AcceptanceCriteria)
                //.Sum();

                projectLevelAcceptanceCriteriaResponse.TotalStories = userStoriesCount;
                projectLevelAcceptanceCriteriaResponse.TotalAcceptanceCriterian = totalAcceptanceCriteria;


                //Pie chart implementation
                double avgClarity = GetAverageClarity();
                var (lowRatio, mediumRatio, highRatio) = GetOverallClarityRatios();

                projectLevelAcceptanceCriteriaResponse.ProjectLevelAcceptanceCriteriaPieChart = new ProjectLevelAcceptanceCriteriaPieChart
                {
                    AverageClarity = avgClarity,
                    AverageCriterianRationForLow = lowRatio,
                    AverageCriterianRationForAverage = avgClarity,
                    AverageCriterianRationForHigh = highRatio
                };

                //Bar chart implementation
                var criteriaBarChart = backlogAcceptanceCriterianResponses.Select(x => new ProjectLevelAcceptanceCriteriaBarChart
                {
                    UserStoryId = x.UserStoryId,
                    Title = x.Title,
                    LowClarityCriterianCount = x.AcceptanceCriteriaJson.AcceptanceCriteria.Count(x => x.Clarity <= 3),
                    AverageClarityCriterianCount = x.AcceptanceCriteriaJson.AcceptanceCriteria.Count(x => x.Clarity == 4),
                    HighClarityCriterianCount = x.AcceptanceCriteriaJson.AcceptanceCriteria.Count(x => x.Clarity == 5),
                    TotalAcceptanceCriteria = x.AcceptanceCriteriaJson.AcceptanceCriteria.Count()

                }).ToList();

                projectLevelAcceptanceCriteriaResponse.ProjectLevelAcceptanceCriteriaBarChart = criteriaBarChart;

                //Top 5 least criterias

                var top5LeastCriterias = backlogAcceptanceCriterianResponses
                    .Select(x => new ProjectLevelAcceptanceCriteriaTopLeast
                    {
                        UserStoryId = x.UserStoryId,
                        Title = x.Title,
                        AcceptanceCriteria = x.AcceptanceCriteriaJson?.AcceptanceCriteria!
                    })
                    .OrderBy(x => x.AcceptanceCriteria.Sum(x => x.Clarity))
                    .Take(5)
                    .ToList();

                projectLevelAcceptanceCriteriaResponse.ProjectLevelAcceptanceCriteriaTopLeast = top5LeastCriterias;

            }
            catch (Exception)
            {
                return null;
            }
            return projectLevelAcceptanceCriteriaResponse;
        }
    }
}



