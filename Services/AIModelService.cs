using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Graph.Models;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GenAiPoc.Core.Interfaces.IService.IVisionetClientService;
using System.Web.Http;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IRepository;
using System.Web.Http.Results;
using Refit;
using System.Net.Http;
using Azure.Core;

namespace GenAiPoc.Application.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly ILogger<AIModelService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IVisionetClientService visionetClientService;
        private readonly IAIModelRepository _modelRepository;

        public AIModelService(ILogger<AIModelService> logger, IConfiguration configuration, IVisionetClientService visionetClientService, IAIModelRepository modelRepository)
        {
            _logger = logger;
            _configuration = configuration;
            this.visionetClientService = visionetClientService;
            this._modelRepository = modelRepository;
        }

        public async Task<Response<AIModelKeyDTO>> GetAIModelApiKeyAsync(GetAiModelApiKeyRequests request)
        {
            var responseModel = new Response<AIModelKeyDTO>();

            try
            {
                // Get the URL from appsettings.json
                string apiUrl = _configuration["AIModelService:AIModelApiKey"];

                if (string.IsNullOrEmpty(apiUrl))
                {
                    _logger.LogError("API URL is not configured in appsettings.");
                    responseModel.Success = false;
                    responseModel.Message = "API URL is missing.";
                    return responseModel;
                }

                var payload = new
                {
                    api_key_id = request.ApiKey
                };

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.Token);

                _logger.LogInformation("Making a POST request to {ApiUrl} with payload: {Payload}", apiUrl, jsonPayload);

                // Send POST request
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read response as string or deserialize to a specific type
                    var contentResponse = await response.Content.ReadAsStringAsync();
                    var modelResponse = JsonConvert.DeserializeObject<AIModelKeyDTO>(contentResponse);

                    responseModel.Success = true;
                    responseModel.Message = "Data retrieved successfully.";
                    responseModel.Data = modelResponse;
                }
                else
                {
                    _logger.LogWarning("API request failed with status code {StatusCode}", response.StatusCode);
                    responseModel.Success = false;
                    responseModel.Message = $"Request failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling the API.");
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching models.";
            }

            return responseModel;
        }

        public async Task<Response<AIModelDTO>> GetAllAIModelsAsync(GetAiModelsRequests request)
        {
            var responseModel = new Response<AIModelDTO>();

            try
            {
                // Get the URL from appsettings.json
                string apiUrl = _configuration["AIModelService:AIModelsUrl"];

                if (string.IsNullOrEmpty(apiUrl))
                {
                    _logger.LogError("API URL is not configured in appsettings.");
                    responseModel.Success = false;
                    responseModel.Message = "API URL is missing.";
                    return responseModel;
                }

                var payload = new
                {
                    user_group = request.UserGroup
                };

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.Token);

                _logger.LogInformation("Making a POST request to {ApiUrl} with payload: {Payload}", apiUrl, jsonPayload);

                // Send POST request
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read response as string or deserialize to a specific type
                    var contentResponse = await response.Content.ReadAsStringAsync();
                    var modelResponse = JsonConvert.DeserializeObject<AIModelDTO>(contentResponse);

                    responseModel.Success = true;
                    responseModel.Message = "Data retrieved successfully.";
                    responseModel.Data = modelResponse;
                }
                else
                {
                    _logger.LogWarning("API request failed with status code {StatusCode}", response.StatusCode);
                    responseModel.Success = false;
                    responseModel.Message = $"Request failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling the API.");
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching models.";
            }

            return responseModel;
        }

        public async Task<Response<AIModelDTO>> GetModelsForAllUserGroupsRequests(GetModelsForAllUserGroupsRequests request)
        {
            var responseModel = new Response<AIModelDTO>();

            try
            {
                // Get the URL from appsettings.json
                string apiUrl = _configuration["AIModelService:AIModelsForAllUserGroupsUrl"];

                if (string.IsNullOrEmpty(apiUrl))
                {
                    _logger.LogError("API URL is not configured in appsettings.");
                    responseModel.Success = false;
                    responseModel.Message = "API URL is missing.";
                    return responseModel;
                }

                var payload = new
                {
                    user_groups = request.UserGroups
                };

                // Serialize the payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.Token);

                _logger.LogInformation("Making a POST request to {ApiUrl} with payload: {Payload}", apiUrl, jsonPayload);

                // Send POST request
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read response as string or deserialize to a specific type
                    var contentResponse = await response.Content.ReadAsStringAsync();
                    var modelResponse = JsonConvert.DeserializeObject<AIModelDTO>(contentResponse);

                    responseModel.Success = true;
                    responseModel.Message = "Data retrieved successfully.";
                    responseModel.Data = modelResponse;
                }
                else
                {
                    _logger.LogWarning("API request failed with status code {StatusCode}", response.StatusCode);
                    responseModel.Success = false;
                    responseModel.Message = $"Request failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling the API.");
                responseModel.Success = false;
                responseModel.Message = "An error occurred while fetching models.";
            }

            return responseModel;

        }

        public async Task<Response<GetUserGroupModelsResponse>> GetAllDynamicModelsAsync(GetDynamicModelRequests request)
        {
            Response<GetUserGroupModelsResponse> response = new Response<GetUserGroupModelsResponse>();
            try
            {
                _logger.LogInformation("Start processing request to retrieve dynamic models for user group. Request: {@Request}", request);

                var result = await visionetClientService.GetAllDynamicModelsAsync(request);

                _logger.LogInformation("Successfully retrieved dynamic models for user group. Response: {@Result}", result);

                response.Success = true;
                response.Message = "Successfully retrieved dynamic models.";
                response.Data = result;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving dynamic models for user group. Request: {@Request}", request);

                response.Success = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            _logger.LogWarning("Returning response with failure. Response: {@Response}", response);
            return response;
        }
        public async Task<Response<GetAllUserGroupModelsResponse>> GetAllUserGroupDynamicModelsAsync(GetModelsForAllGroupsRequest request)
        {
            //logg
            Response<GetAllUserGroupModelsResponse> response = new Response<GetAllUserGroupModelsResponse>();
            try
            {
                _logger.LogInformation("Start processing request to retrieve all user group dynamic models. Request: {@Request}", request);

                var result = await visionetClientService.GetAllUserGroupDynamicModelsAsync(request);
                _logger.LogInformation("Successfully retrieved models for all user groups. Response: {@Result}", result);

                response.Success = true;
                response.Message = "Successfully retrieved models";
                response.Data = result;
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during API call. Request: {@Request}", request);
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            _logger.LogWarning("Returning response with failure. Response: {@Response}", response);
            return response;
        }
        public async Task<Response<GetAllUserGroupModelsResponse>> GetAllUserGroupDynamicModelsV2Async(GetModelsForAllGroupsByUsernameRequest request)
        {
            //logg
            Response<GetAllUserGroupModelsResponse> response = new Response<GetAllUserGroupModelsResponse>();
            try
            {
                _logger.LogInformation("Start processing request to retrieve all user group dynamic models. Request: {@Request}", request);

                var result = await visionetClientService.GetAllUserGroupDynamicModelsV2Async(request);
                _logger.LogInformation("Successfully retrieved models for all user groups. Response: {@Result}", result);

                response.Success = true;
                response.Message = "Successfully retrieved models";
                response.Data = result;
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during API call. Request: {@Request}", request);
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            _logger.LogWarning("Returning response with failure. Response: {@Response}", response);
            return response;
        }
        public async Task<Response<bool>> AddModelVersionDetailAsync(AddModelVersionDetailDto request)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("AddModelVersionDetailAsync was called with a null request.");
                    return new Response<bool>
                    {
                        Success = false,
                        Message = "Request can not be empty."
                    };
                }

                var modelVersionDetail = new ModelVersionDetail
                {
                    Name = request.Name,
                    Version = request.Version,
                    Description = request.Description,
                    CreatedDate = DateTime.UtcNow
                };

                var result =  await _modelRepository.AddModelVersionDetail(modelVersionDetail);

                if (!result)
                {
                    _logger.LogWarning($"Failed to create ModelVersionDetail with Name: {request.Name}");
                    response.Message = "Failed to create the model version detail.";
                    response.Success = false;

                    return response;
                }

                return new Response<bool>
                {
                    Success = true,
                    Message = "Operation completed successfully."
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new ModelVersionDetail.");
                return new Response<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseList<ModelVersionDetail>> GetModelVersionDetailsAsync()
        {
            try
            {
                var result = await _modelRepository.GetModelVersionDetails();

                return new ResponseList<ModelVersionDetail>
                {
                    Data = result,
                    Success = true,
                    Status = 1,
                    Message = "Data retrieved successfully."
                };
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching model version details.");
                return new ResponseList<ModelVersionDetail>
                {
                    Success = false,
                    Status = 0,
                    Message = ex.Message
                };
            }
        }

        public async IAsyncEnumerable<string> ModelResponseStream(ChatHistoryRequest request)
        {
            var customBaseUrl = "https://app-codeanalyzer-ocelot.azurewebsites.net";
            //var customBaseUrl = "http://localhost:7239";

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(customBaseUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.AccessToken);

            var customClient = RestService.For<IVisionetClientService>(httpClient);

            //var customClient = RestService.For<IVisionetClientService>(new HttpClient
            //{
            //    BaseAddress = new Uri(customBaseUrl)
            //});

            var stream = await customClient.QueryChatStream(request);

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    yield return line;
                }
            }
        }
    }
}
