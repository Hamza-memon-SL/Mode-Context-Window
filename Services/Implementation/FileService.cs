using Amazon.Runtime.Internal;
using Aspose.Svg.Net;
using Azure;
using Azure.Core;
using Function.HttpTriggerChat.Domain.DTO;
using Function.HttpTriggerChat.Domain.Request;
using Function.HttpTriggerChat.Domain.Response;
using Function.HttpTriggerChat.Service.Interface;
using GenAiPoc.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Package.AzureOpenAi;
using Package.ClaudeAi;
using Package.CustomAiModel;
using Package.CustomAiModel.DTO;
using Package.Database.Entities;
using Package.Database.Enum;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Function.HttpTriggerChat.Service.Implementation
{
    public class FileService : IFileService
    {
        private readonly DbContextGenAiPOC _dbContext;
        private readonly ILogger<FileService> _logger;
        private readonly ClaudeAiService _claudeAiService;
        private readonly CustomAiModelService _customAiModel;
        private readonly OpenAiService _openAiService;

        public FileService(ILogger<FileService> logger, DbContextGenAiPOC dbContext, OpenAiService openAiService, ClaudeAiService claudeAiService, CustomAiModelService customAiModel)
        {
            _logger = logger;
            _dbContext = dbContext;
            _claudeAiService = claudeAiService;
            this._customAiModel = customAiModel;
            _openAiService = openAiService;
        }
        public async Task<string> GetUserInterfaceV2(UserInterfaceRequest request)
        {
            try
            {
                ChatHistory chatHistory = new ChatHistory();
                List<string> aiResponsesText = new List<string>();
                string response = string.Empty;
                var userStoryTemplate = string.Empty;
                string prompt = string.Format(@"Return just a html page interface for {0}. The response should have just HTML, css and JS content no text.", request.Query);
                var customModelDtoText = new CustomModelDto
                {
                    Content = prompt,
                    AuthToken = request.AuthToken,
                    UserName = request.UserName,
                    ModelDetail = request.ModelDetail,
                    AccessToken = request?.ModelDetail?.Token
                };
                _logger.LogInformation($"Sending File Content to Custom Model for Text: Query = {request.Query}");
                aiResponsesText = await _customAiModel.SendContentToModelAsync(customModelDtoText);
                // aiResponsesText = await _openAiService.SendContentToAzureOpenAI(prompt, string.Empty, request.AuthToken, request.UserName);
                if (aiResponsesText != null && aiResponsesText.Count > 0)
                {
                    chatHistory.Content = prompt;
                    chatHistory.Answer = aiResponsesText[0];
                    chatHistory.CreatedTime = DateTime.UtcNow;
                    chatHistory.ChatType = (int)EnumChatType.UICreationQuery;
                    chatHistory.AuthToken = request.AuthToken;
                    _dbContext.ChatHistory.Add(chatHistory);
                    await _dbContext.SaveChangesAsync();
                }
                return chatHistory.Answer;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<string> GetUserInterface(UserInterfaceRequest request)
        {
            try
            {
                ChatHistory chatHistory = new ChatHistory();
                List<string> aiResponsesText = new List<string>();
                string response = string.Empty;
                var userStoryTemplate = string.Empty;
                string prompt = string.Format(@"Return just a html page interface for {0}. The response should have just HTML, css and JS content no text.", request.Query);
                aiResponsesText = await _openAiService.SendContentToAzureOpenAI(prompt, string.Empty, request.AuthToken, request.UserName);
                if (aiResponsesText != null && aiResponsesText.Count > 0)
                {
                    chatHistory.Content = prompt;
                    chatHistory.Answer = aiResponsesText[0];
                    chatHistory.CreatedTime = DateTime.UtcNow;
                    chatHistory.ChatType = (int)EnumChatType.UICreationQuery;
                    chatHistory.AuthToken = request.AuthToken;
                    _dbContext.ChatHistory.Add(chatHistory);
                    await _dbContext.SaveChangesAsync();
                }
                return chatHistory.Answer;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<string> AnalyzeImage(UserInterfaceRequest request)
        {
            try
            {
                ChatHistory chatHistory = new ChatHistory();
                List<string> aiResponsesText = new List<string>();

                if (string.IsNullOrEmpty(request.Query))
                {
                    request.Query = "Please Convert the image into html and css code The response must include only the HTML and CSS, with no additional text or commentary and please remove the html comment at start and the end.";
                }
                else

                {
                    request.Query = string.Format("Generate an html, css page for the following front-end {0}. the response must include only the HTML and CSS, with no additional text or commentary and please remove the html comment at start and the end.", request.Query);
                }
                //var customModelDtoText = new CustomModelDto
                //{
                //    Content = request.Query+ request?.FileBase64 ?? string.Empty,
                //    AuthToken = request.AuthToken,
                //    UserName = request.UserName,
                //    ModelDetail = request.ModelDetail,
                //    AccessToken = request?.ModelDetail?.Token
                //};
                //_logger.LogInformation($"Sending File Content to Custom Model for Text: FileId = {request.Query}");
                //aiResponsesText = await _customAiModel.SendContentToModelAsync(customModelDtoText);
                aiResponsesText = await _openAiService.SendImageContentToAzureOpenAI(request.Query, string.Empty, request?.FileBase64 ?? string.Empty, request.AuthToken, request.UserName);

                if (aiResponsesText != null && aiResponsesText.Count > 0)
                {
                    chatHistory.Content = request.Query;
                    chatHistory.Answer = aiResponsesText[0];
                    chatHistory.CreatedTime = DateTime.UtcNow;
                    chatHistory.ChatType = (int)EnumChatType.ImageQuery;
                    chatHistory.AuthToken = request.AuthToken;
                    _dbContext.ChatHistory.Add(chatHistory);
                    await _dbContext.SaveChangesAsync();
                }
                return chatHistory.Answer;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<string> GetUserStory(UserStoryRequest question)
        {
            try
            {
                ChatHistory chatHistory = new ChatHistory();
                List<string> aiResponsesText = new List<string>();
                // string response = string.Empty;
                var userStoryTemplatePrompt = string.Empty;
                var configuration = await _dbContext.Configurations.AsNoTracking().FirstOrDefaultAsync();
                if (question.Simple)
                {
                    userStoryTemplatePrompt = string.Format(@"{0} in the following format {1}", question.Query, configuration?.SimpleUserStoryTemplate);
                }
                else
                {
                    userStoryTemplatePrompt = string.Format(@"{0} in the following format {1} ready for developer to start work. The response should not include code just steps to work on.", question.Query, configuration?.ComplexUserStoryTemplate);
                }
                var customModelDtoText = new CustomModelDto
                {
                    Content = userStoryTemplatePrompt,
                    AuthToken = question.AuthToken,
                    UserName = question.UserName,
                    ModelDetail = question.ModelDetail,
                    AccessToken = question.ModelDetail.Token
                };
                _logger.LogInformation($"Sending File Content to Custom Model for Text: FileId = {question.ChatType}");
                aiResponsesText = await _customAiModel.SendContentToModelAsync(customModelDtoText);
                // aiResponsesText = await _openAiService.SendContentToAzureOpenAI(userStoryTemplatePrompt, string.Empty, question.AuthToken, question.UserName);
                // response = aiResponsesText[0];
                if (aiResponsesText != null && aiResponsesText.Count > 0)
                {
                    chatHistory.Content = question.Query;
                    chatHistory.Answer = aiResponsesText[0];
                    chatHistory.CreatedTime = DateTime.UtcNow;
                    chatHistory.ChatType = (int)EnumChatType.UserStoryCreationQuery;
                    chatHistory.AuthToken = question.AuthToken;
                    _dbContext.ChatHistory.Add(chatHistory);
                    await _dbContext.SaveChangesAsync();
                }
                return chatHistory.Answer;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<List<ChatHistory>> Query(ChatHistoryDTO question)
        {
            try
            {
                ChatHistory chatHistory = new ChatHistory();
                List<string> aiResponsesText = new List<string>();

                var chatPrompt = await _dbContext.ChatPrompts.Where(x => x.Id == question.PromptId).AsNoTracking().FirstOrDefaultAsync();
                if (question.WorkspaceId == 0)
                {
                    _logger.LogInformation($"Query question sending to Open Ai: " + question.AuthToken);

                    string questionToLLM = string.Format("{0} \n\n {1}", chatPrompt?.PromptText, question.Content);
                    aiResponsesText = await _openAiService.SendContentToAzureOpenAI(questionToLLM, string.Empty, question.AuthToken, question.UserName);
                }
                else
                {
                    var workSpace = await _dbContext.CodeBuddyProjectSettings.Where(x => x.ProjectId == question.WorkspaceId).AsNoTracking().FirstOrDefaultAsync();

                    if (workSpace == null)
                    {
                        return new List<ChatHistory>();
                    }
                    if (workSpace?.ModelId == 1)
                    {
                        _logger.LogInformation($"Query question sending to Open Ai: " + question.AuthToken);

                        string questionToLLM = string.Format("{0} \n\n {1}", chatPrompt?.PromptText, question.Content);

                        aiResponsesText = await _openAiService.SendContentToAzureOpenAI(questionToLLM, string.Empty, question.AuthToken, question.UserName);
                    }
                    else if (workSpace?.ModelId == 2)
                    {
                        _logger.LogInformation($"Query question sending to Open Ai: " + question.AuthToken);

                        string questionToLLM = string.Format("{0} \n\n{1}", chatPrompt?.PromptText, question.Content);

                        aiResponsesText = await _claudeAiService.SendContentToClaudeAi(questionToLLM, string.Empty, question.AuthToken, question.UserName);

                    }
                    chatHistory.WorkspaceId = workSpace?.ProjectId ?? 0;
                    chatHistory.PromptId = chatPrompt?.Id ?? 0;
                }

                if (aiResponsesText != null && aiResponsesText.Count > 0)
                {
                    chatHistory.Answer = aiResponsesText[0];
                    chatHistory.Content = question.Content;
                    chatHistory.AuthToken = question.AuthToken;
                    chatHistory.UserName = question.UserName;
                    chatHistory.CreatedTime = DateTime.UtcNow;
                    chatHistory.ChatType = (int)EnumChatType.ChatQuery;
                    chatHistory.HistoryChatType = question.HistoryChatType;
                    _dbContext.ChatHistory.Add(chatHistory);
                    await _dbContext.SaveChangesAsync();
                }
                IQueryable<ChatHistory> query;
                if (question.WorkspaceId == 0)
                {
                    query = _dbContext.ChatHistory
                        .Where(x => x.AuthToken == question.AuthToken);
                }
                else
                {
                    query = _dbContext.ChatHistory
                        .Where(x => x.WorkspaceId == question.WorkspaceId);
                }
                var resultHistory = await query
                    .OrderByDescending(x => x.CreatedTime)
                    .AsNoTracking()
                       .Select(x => new ChatHistory
                       {
                           Id = x.Id,
                           PromptId = x.PromptId,
                           Content = x.Content,
                           WorkspaceId = x.WorkspaceId,
                           AuthToken = x.AuthToken,
                           Answer = x.Answer,
                           UserName = x.UserName,
                           CreatedTime = ConvertUTCtoPST(x.CreatedTime),
                           ChatType = x.ChatType,
                           HistoryChatType = x.HistoryChatType
                       })
                    .ToListAsync();
                return resultHistory;
            }
            catch (Exception ex)
            {
                throw;

            }
        }
        public async Task<string> MainFrameSimpleCodeSummarize(ChatHistoryRequest question)
        {
            try
            {
            
                string questionToLLM = string.Empty;
                var aiResponsesText = await _customAiModel.SendContentToModelAsync(question.ToCustomModelDto());
                return aiResponsesText[0];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MainFrameSimpleCodeSummarize");
                throw;
            }
        }
        public async Task<List<ChatHistory>> QueryV2(ChatHistoryRequest question)
        {
            try
            {
                string questionToLLM = string.Empty;

                var chatPrompt = await _dbContext.ChatPrompts
                    .Where(x => x.Id == question.PromptId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var chatHistories = new List<ChatHistory>();

                // Add condition to return empty result when ChatSessionId is 0
                if (question.ChatSessionId != 0)
                {
                    chatHistories = await _dbContext.ChatHistory
                        .Where(x => x.AuthToken == question.AuthToken && x.ChatSessionId == question.ChatSessionId)
                        .OrderBy(x => x.Id)
                        .Take(1)
                        .Union(
                            _dbContext.ChatHistory
                            .Where(x => x.AuthToken == question.AuthToken && x.ChatSessionId == question.ChatSessionId)
                            .OrderByDescending(x => x.Id)
                            .Take(2)
                        )
                        .OrderBy(x => x.Id)
                        .ToListAsync();
                }

                // If ChatSessionId is 0, return an empty list (effectively count will be 0)
                if (question.ChatSessionId == 0)
                {
                    chatHistories = new List<ChatHistory>(); // Set it to an empty list
                }


                var chatHistoryContent = string.Join(Environment.NewLine,
                    chatHistories.Select(x => $"{x.Content}{Environment.NewLine}{Environment.NewLine}{x.Answer}"));

                _logger.LogInformation($"Query question sending to LLM Mode : {question.AuthToken}");
                questionToLLM = chatPrompt != null && !string.IsNullOrEmpty(chatPrompt.PromptText) ? string.Format("{0}\n\n{1}", chatPrompt.PromptText, question.Content) : question.Content;

                question.Content = string.IsNullOrEmpty(chatHistoryContent)
                    ? questionToLLM
                    : chatHistoryContent + Environment.NewLine + Environment.NewLine + Environment.NewLine + questionToLLM;

                var aiResponsesText = await _customAiModel.SendContentToModelAsync(question.ToCustomModelDto());
                if (aiResponsesText == null || aiResponsesText.Count == 0)
                {
                    return new List<ChatHistory>();
                }

                var chatHistory = new ChatHistory
                {
                    Answer = aiResponsesText[0],
                    Content = questionToLLM,
                    AuthToken = question.AuthToken,
                    UserName = question.UserName,
                    CreatedTime = DateTime.UtcNow,
                    WorkspaceId = question.WorkspaceId,
                    PromptId = chatPrompt?.Id ?? 0,
                    ChatType = question.ChatType,
                    HistoryChatType = question.HistoryChatType,
                    ModelName = question.ModelDetail.ModelName,
                    ChatSessionId = question.ChatSessionId == 0
                        ? (await _dbContext.ChatHistory
                            .Where(c => c.AuthToken == question.AuthToken)
                            .Select(c => (int?)c.ChatSessionId)
                            .MaxAsync() ?? 0) + 1
                        : question.ChatSessionId,
                };

                _dbContext.ChatHistory.Add(chatHistory);
                await _dbContext.SaveChangesAsync();

                var query = _dbContext.ChatHistory
                    .Where(x => x.AuthToken == question.AuthToken && (question.WorkspaceId == 0 || x.WorkspaceId == question.WorkspaceId))
                    .OrderByDescending(x => x.CreatedTime);

                return await query
                    .AsNoTracking()
                    .Select(x => new ChatHistory
                    {
                        Id = x.Id,
                        PromptId = x.PromptId,
                        Content = x.Content,
                        WorkspaceId = x.WorkspaceId,
                        AuthToken = x.AuthToken,
                        Answer = x.Answer,
                        UserName = x.UserName,
                        CreatedTime = ConvertUTCtoPST(x.CreatedTime),
                        ChatType = x.ChatType,
                        HistoryChatType = x.HistoryChatType,
                        ChatSessionId = x.ChatSessionId
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QueryV2");
                throw;
            }
        }

        public async IAsyncEnumerable<string> QueryV2Stream(ChatHistoryRequest question)
        {
            string questionToLLM = string.Empty;
            var chatPrompt = await _dbContext.ChatPrompts
                .Where(x => x.Id == question.PromptId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var chatHistories = new List<ChatHistory>();

            if (question.ChatSessionId != 0)
            {
                chatHistories = await _dbContext.ChatHistory
                    .Where(x => x.AuthToken == question.AuthToken && x.ChatSessionId == question.ChatSessionId)
                    .OrderBy(x => x.Id)
                    .Take(1)
                    .Union(
                        _dbContext.ChatHistory
                        .Where(x => x.AuthToken == question.AuthToken && x.ChatSessionId == question.ChatSessionId)
                        .OrderByDescending(x => x.Id)
                        .Take(2)
                    )
                    .OrderBy(x => x.Id)
                    .ToListAsync();
            }

            if (question.ChatSessionId == 0)
            {
                chatHistories = new List<ChatHistory>();
            }

            var chatHistoryContent = string.Join(Environment.NewLine,
                chatHistories.Select(x => $"{x.Content}{Environment.NewLine}{Environment.NewLine}{x.Answer}"));

            _logger.LogInformation($"Query question sending to LLM Mode : {question.AuthToken}");
            questionToLLM = chatPrompt != null && !string.IsNullOrEmpty(chatPrompt.PromptText)
                ? string.Format("{0}\n\n{1}", chatPrompt.PromptText, question.Content)
                : question.Content;

            question.Content = string.IsNullOrEmpty(chatHistoryContent)
                ? questionToLLM
                : chatHistoryContent + Environment.NewLine + Environment.NewLine + Environment.NewLine + questionToLLM;

            var fullResponse = new StringBuilder();

            await foreach (var responseChunk in _customAiModel.SendContentToModelStreamAsync(question.ToCustomModelDto()))
            {
                fullResponse.Append(responseChunk);
                yield return responseChunk; // Stream response chunk to frontend
            }

            // After streaming is done, save the full response in the database
            var chatHistory = new ChatHistory
            {
                Answer = fullResponse.ToString(),
                Content = questionToLLM,
                AuthToken = question.AuthToken,
                UserName = question.UserName,
                CreatedTime = DateTime.UtcNow,
                WorkspaceId = question.WorkspaceId,
                PromptId = chatPrompt?.Id ?? 0,
                ChatType = question.ChatType,
                HistoryChatType = question.HistoryChatType,
                ModelName = question.ModelDetail.ModelName,
                ChatSessionId = question.ChatSessionId == 0
                    ? (await _dbContext.ChatHistory
                        .Where(c => c.AuthToken == question.AuthToken)
                        .Select(c => (int?)c.ChatSessionId)
                        .MaxAsync() ?? 0) + 1
                    : question.ChatSessionId,
            };

            _dbContext.ChatHistory.Add(chatHistory);
            await _dbContext.SaveChangesAsync();

            var sessionId = chatHistory.ChatSessionId;

            yield return "[EOD]-"+sessionId;
        }



        public async Task<GetGlobalAnalytics> GetGlobalAnalyticsAsync()
        {
            GetGlobalAnalytics getGlobalAnalytics = new GetGlobalAnalytics();
            try
            {
                var projects = await _dbContext.CodeBuddyProjects
                 .GroupBy(p => true)
                 .Select(g => new
                 {
                     RegisteredUsers = g.Select(p => p.AuthToken).Distinct().Count(),
                     ProjectImported = g.Count()
                 })
                 .FirstOrDefaultAsync();

                var searches = await _dbContext.ChatHistory.CountAsync();
                var customizationTimeSaved = await GetCustomizationTimeSaved();

                var hoursSaved = await CalculateEstimatedTimeSavedByDbForWeb
                    (_dbContext.CodeSuggestions.Sum(x => x.NumberOfLinesCoppied),
                    customizationTimeSaved.EstimatedTimeSavedFormulaValue,
                    customizationTimeSaved.EstimatedTimeSavedDashboardFormat);
                string decimalHoursSaved = "";
                if (!string.IsNullOrEmpty(hoursSaved))
                {
                    decimalHoursSaved = await ConvertHoursSavedToDecimal(hoursSaved);
                }
                //hoursSaved = (hoursSaved / 250)/60;
                var costSaved = 50 * Convert.ToDecimal(decimalHoursSaved);


                getGlobalAnalytics.ProjectImported = projects?.ProjectImported;
                getGlobalAnalytics.RegisteredUsers = projects?.RegisteredUsers;
                getGlobalAnalytics.SearchPerformed = searches;
                getGlobalAnalytics.EstimatedCostSaved = costSaved;
                getGlobalAnalytics.EstimatedTimeSaved = hoursSaved;


                return getGlobalAnalytics;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<GetUserSpecificAnalytics> GetUserSpecificAnalyticsAsync(string authToken)
        {
            GetUserSpecificAnalytics getUserSpecificAnalytics = new GetUserSpecificAnalytics();
            try
            {

                var projectsImported = await _dbContext.CodeBuddyProjects.Where(x => x.AuthToken == authToken).CountAsync();
                var searches = await _dbContext.ChatHistory.Where(x => x.AuthToken == authToken).CountAsync();
                var customizationTimeSaved = await GetCustomizationTimeSaved();

                var hoursSaved = await CalculateEstimatedTimeSavedByDbForWeb
                    (_dbContext.CodeSuggestions.Where(x => x.AuthToken == authToken).Sum(x => x.NumberOfLinesCoppied),
                    customizationTimeSaved.EstimatedTimeSavedFormulaValue,
                    customizationTimeSaved.EstimatedTimeSavedDashboardFormat);
                string decimalHoursSaved = "";
                if (!string.IsNullOrEmpty(hoursSaved))
                {
                    decimalHoursSaved = await ConvertHoursSavedToDecimal(hoursSaved);
                }
                //hoursSaved = (hoursSaved / 250) / 60;
                var costSaved = 50 * Convert.ToDecimal(decimalHoursSaved);

                getUserSpecificAnalytics.ProjectImported = projectsImported > 0 ? projectsImported : 0;
                getUserSpecificAnalytics.SearchPerformed = searches > 0 ? searches : 0;
                getUserSpecificAnalytics.EstimatedCostSaved = costSaved;
                getUserSpecificAnalytics.EstimatedTimeSaved = hoursSaved;


                return getUserSpecificAnalytics;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<MainFrameDestinationFiles> GetMainFrameDestinationFileDetailById(int fileId)
        {
            _logger.LogInformation("Get file details by id method Start.");
            try
            {
                var destinationFiles = await _dbContext.MainFrameDestinationFiles.FirstOrDefaultAsync(x => x.Id.Equals(fileId));

                return destinationFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get file details by id method error.");
                throw;
            }
        }
        public async Task<string> ReadFileContent(string filePaths)
        {
            _logger.LogInformation("Reading File Content method start");
            try
            {
                string fileContent = "";
                using (HttpClient httpClient = new HttpClient())
                {
                    fileContent = await httpClient.GetStringAsync(filePaths);
                }

                return fileContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading files: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateMainFrameDestinationFileDetailsAsync(string summary, MainFrameDestinationFiles fileDetails)
        {
            _logger.LogInformation("Updating the Ai reponse to summry in db");
            try
            {
                fileDetails.Summary = summary;
                fileDetails.IndexDate = DateTime.UtcNow;
                fileDetails.Status = SummarizeMainFrame.Summarized;
                _dbContext.MainFrameDestinationFiles.Update(fileDetails);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in Bulk Update" + ex.Message);
            }


        }
        public async Task<MainFrameConfigurations> GetMainFrameConfigurations()
        {
            try
            {
                _logger.LogInformation("Getting mainFrame configurations from db");

                return await _dbContext.MainFrameConfigurations
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in in getting configuration" + ex.Message);
                throw;
            }
        }
        public async Task<Configurations> GetConfigurations()
        {
            try
            {
                _logger.LogInformation("Getting configurations from db");

                return await _dbContext.Configurations.AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in in getting configuration" + ex.Message);
                throw;
            }
        }
        public async Task<string> GetJsonFromResponse(string aiResponse)
        {
            return await Task.Run(() =>
            {
                int jsonStartIndex = aiResponse.IndexOf("json") + "json".Length;
                int jsonEndIndex = aiResponse.LastIndexOf("```");

                if (jsonStartIndex != -1 && jsonEndIndex != -1)
                {
                    string json = aiResponse.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex).Trim();


                    return json;
                }

                return aiResponse; // or throw an exception, depending on your requirements
            });
        }
        public async Task<List<ArtifactCategory>> GetArtifactCategoryAsync()
        {
            try
            {
                var artifactCategory = await _dbContext.ArtifactCategories
                                                .Include(x => x.ArtifactSections)
                                                    .Include(x => x.ArtifactEvaluationCriterias).ToListAsync();

                return artifactCategory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> CreateArtifactGeneratorUserStoryAsync(CreateArtifactGeneratorUserStoryDTO dto)
        {
            try
            {
                List<string> aiResponsesText = new List<string>();

                dto.Prompt = "Please generate a user story with the following details : \n " + dto.Prompt;

                var customModelDtoText = new CustomModelDto
                {
                    Content = dto.Prompt,
                    AuthToken = dto.AuthToken,
                    UserName = dto.UserName,
                    ModelDetail = dto.ModelDetail,
                    AccessToken = dto.ModelDetail.Token
                };

                aiResponsesText = await _customAiModel.SendContentToModelAsync(customModelDtoText);
     
                return aiResponsesText[0];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private async Task<string> ConvertHoursSavedToDecimal(string hoursSaved)
        {
            //var match = System.Text.RegularExpressions.Regex.Match(hoursSaved, @"(\d+)\s*hour(?:s)?(?:\s*and\s*(\d+)\s*minute(?:s)?)?");
            var match = System.Text.RegularExpressions.Regex.Match(hoursSaved, @"(?:(\d+)\s*hour(?:s)?)?(?:\s*and\s*)?(?:(\d+)\s*minute(?:s)?)?");
            if (match.Success)
            {
                int hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
                int minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;

                if (hours == 0 && minutes == 0)
                {
                    return "0";
                }
                //return hours + (minutes / 60.0);
                return hours + "." + minutes;

            }
            else
            {
                throw new FormatException("Invalid time format.");
            }
        }
        private async Task<string> CalculateEstimatedTimeSavedByDbForWeb(int x, string formula, string format)
        {

            formula = formula.Replace("{0}", x.ToString());

            double Y = (double)new System.Data.DataTable().Compute(formula, string.Empty);

            double totalMinutes = Y;
            int totalSeconds = (int)(totalMinutes * 60);

            int days = totalSeconds / (24 * 3600);
            int remainingSecondsAfterDays = totalSeconds % (24 * 3600);

            int hours = remainingSecondsAfterDays / 3600;
            int remainingSecondsAfterHours = remainingSecondsAfterDays % 3600;

            int minutes = remainingSecondsAfterHours / 60;
            int seconds = remainingSecondsAfterHours % 60;

            double decimalHours = (double)totalSeconds / 3600;

            var timeValues = new Dictionary<string, string>
                  {
                      { "{dd}", days.ToString("D2") },
                      { "{d}", days.ToString("D2") },
                      { "{hh}", hours.ToString("D2") },
                      { "{h}", hours.ToString("D2") },
                      { "{mm}", minutes.ToString("D2") },
                      { "{m}", minutes.ToString("D2") },
                      { "{ss}", seconds.ToString("D2") },
                      { "{s}", seconds.ToString("D2") },
                      { "{hh.mm}", decimalHours.ToString("F2") },
                      { "{ms}", (totalSeconds * 1000).ToString() },
                      { "{hh:mm:ss}", $"{hours:D2}:{minutes:D2}:{seconds:D2}" },
                      { "{hh}h {mm}m", $"{hours:D2}h {minutes:D2}m" },
                      { "{hh} hrs {mm} min", $"{hours:D2} hrs {minutes:D2} min" },
                      { "{mm}:{ss}", $"{minutes:D2}:{seconds:D2}" }
                  };
            format = timeValues.Aggregate(format, (current, timeValue) =>
                 current.Contains(timeValue.Key) ? current.Replace(timeValue.Key, timeValue.Value) : current);
            return format;
        }
        private async Task<CustomizationTimeSaved> GetCustomizationTimeSaved()
        {
            var customization = await _dbContext.CustomizationTimeSaved.Where(x => x.IsActive == true).FirstOrDefaultAsync();
            return customization;
        }
        private static DateTime ConvertUTCtoPST(DateTime dateTime)
        {
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, pstZone);
        }

    }
}
