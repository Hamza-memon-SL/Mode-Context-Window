using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using GenAiPoc.Contracts.Models;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Mapping;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using Microsoft.Graph.Models;
using System.Net.WebSockets;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using System.Net.Http.Headers;

namespace GenAiPoc.Application.Services
{
    public class CodeBuddyService : ICodeBuddyService
    {
        private readonly ICodeBuddyRepository _codeBuddyRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        public CodeBuddyService(ICodeBuddyRepository codeBuddyRepository, IBlobStorageService blobStorageService, IMapper mapper)
        {
            _codeBuddyRepository = codeBuddyRepository;
            _blobStorageService = blobStorageService;
            _mapper = mapper;
        }

        #region  //local disk
        public async Task<ImportResponse> ImportFile(ImportFileDTO request)
        {

            try
            {
                ImportResponse response;

                var project = await _codeBuddyRepository.GetProjectByRepoUrl(request);
                if (project == null)
                {
                    if (request.IsInMemory)
                    {
                        var repo = await _codeBuddyRepository.IsDevopsRepoExist(request);
                        if (repo.Status == 0)
                        {
                            response = await _codeBuddyRepository.ProcessDevopsRepository(request, repo.Item);

                            return new ImportResponse(response.Status, response.Message);

                        }
                        else
                        {
                            return new ImportResponse(repo.Status, repo.Item);
                        }

                    }
                    else
                    {
                        var localPath = await _codeBuddyRepository.GetTempFolderPath();
                        (bool isSuccess, bool isPrivate, bool isBranchExist, string msg) cloneSuccess = (false, request.isPrivate, false, "");
                        if (request.isPrivate == false)
                            cloneSuccess = await _codeBuddyRepository.ClonePublicRepository(request, localPath);
                        else if (request.isPrivate == true)
                            cloneSuccess = await _codeBuddyRepository.ClonePrivateRepository(request, localPath);
                        if (cloneSuccess.isSuccess == true)
                        {
                            response = await _codeBuddyRepository.ProcessRepositoryAsync(request, localPath);

                            if (response.Status == 0)
                            {
                                return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, response.Message);
                            }
                            else
                            {
                                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, response.Message);
                            }
                        }
                        else if (cloneSuccess.isSuccess == false && cloneSuccess.isPrivate == true)
                        {
                            return new ImportResponse(StatusAndMessagesKeys.PrivateRepositoryError, StatusAndMessagesKeys.PrivateRepository + " " + cloneSuccess.msg);
                        }
                        else
                        {
                            return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.Workspacecloningfailed + " " + cloneSuccess.msg);
                        }

                    }
                }
                else
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAlreadyExist);
                }


            }
            catch (Exception ex)
            {
                return new ImportResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }

        }

        public async Task<ImportResponse> CreateProject(ImportFileDTO request)
        {
            ImportResponse response;
            try
            {
                var project = await _codeBuddyRepository.GetProjectByRepoUrl(request);

                if(project != null)
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAlreadyExist);
                }

                bool isAccessible = await IsRepositoryAccessible(request.GitHubURL, request.UserName, request.Password);
                if (!isAccessible)
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, "Invalid URL or unable to access the specified repository.");
                }


                var localPath = await _codeBuddyRepository.GetTempFolderPath();
                var repoName = await _codeBuddyRepository.GetRepositoryName(request.GitHubURL);
                var repoPath = Path.Combine(localPath, repoName);
                var projectAdded = await _codeBuddyRepository.ImportFileWithoutCheckSum(request, request.GitHubURL, null);

                if (projectAdded != null && projectAdded.Status == StatusAndMessagesKeys.SuccessStatus)
                {
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, projectAdded.Message);
                }

                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, "Invalid URL or unable to access the specified URL.");

            }
            catch (Exception ex)
            {

                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, ex.Message);
            }
        }

        //private async Task<bool> IsRepositoryAccessible(string repoUrl, string username, string token)
        //{
        //    try
        //    {
        //        using var httpClient = new HttpClient();
        //        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

        //        // Add authentication if credentials are provided (for private repos)
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{token}"));
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        //        }

        //        // Extract the owner and repo name from the URL
        //        var repoPath = repoUrl.Replace("https://github.com/", "").Replace(".git", "");
        //        var apiUrl = $"https://api.github.com/repos/{repoPath}";

        //        var response = await httpClient.GetAsync(apiUrl);

        //        return response.IsSuccessStatusCode; // Returns true if repo is accessible
        //    }
        //    catch
        //    {
        //        return false; // If any error occurs, consider repo inaccessible
        //    }
        //}

        private async Task<bool> IsRepositoryAccessible(string repoUrl, string username, string token)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                string projectType = GetProjectType(repoUrl);
                string apiUrl = string.Empty;

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = projectType == "GitLab"
                        ? new AuthenticationHeaderValue("Bearer", token)  // GitLab requires Bearer Token
                        : new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{token}")));
                }

                switch (projectType)
                {
                    case "GitHub":
                        var githubRepoPath = repoUrl.Replace("https://github.com/", "").Replace(".git", "");
                        apiUrl = $"https://api.github.com/repos/{githubRepoPath}";
                        break;

                    case "GitLab":
                        var gitlabRepoPath = repoUrl.Replace("https://gitlab.com/", "").Replace(".git", "");
                        var encodedGitLabPath = Uri.EscapeDataString(gitlabRepoPath);
                        apiUrl = $"https://gitlab.com/api/v4/projects/{encodedGitLabPath}";
                        break;

                    case "Bitbucket":
                        var bitbucketRepoPath = repoUrl.Replace("https://bitbucket.org/", "").Replace(".git", "");
                        apiUrl = $"https://api.bitbucket.org/2.0/repositories/{bitbucketRepoPath}";
                        break;

                    case "Azure DevOps":
                        //var azureRepoPath = repoUrl.Replace("https://dev.azure.com/", "").Replace(".git", "");
                        //apiUrl = $"https://dev.azure.com/{azureRepoPath}/_apis/git/repositories?api-version=6.0";

                        var urlComponent =  await _codeBuddyRepository.GetRepositoryInfoFromUrl(repoUrl);
                        apiUrl = _codeBuddyRepository.GetDevopsUrl(urlComponent);


                        var chk = await _codeBuddyRepository.IsDevopsRepoExist(new ImportFileDTO { GitHubURL = repoUrl, UserName = username, Password = token });
                        break;

                    default:
                        return false; // Unsupported or unknown provider
                }

                var response = await httpClient.GetAsync(apiUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // If any error occurs, consider repo inaccessible
            }
        }

        private string GetProjectType(string url)
        {
            if (url.Contains("github.com"))
            {
                return "GitHub";
            }
            else if (url.Contains("gitlab.com") || url.Contains("gitlab"))
            {
                return "GitLab";
            }
            else if (url.Contains("bitbucket.org"))
            {
                return "Bitbucket";
            }
            else if (url.Contains("dev.azure.com") || url.Contains(".visualstudio.com"))
            {
                return "Azure DevOps";
            }
            else
            {
                return "Unknown or Self-Hosted";
            }
        }

        #endregion
        public async Task<GetWorkspaceResponse> GetWorkspaceAsync(int ProjectId)
        {

            try
            {
                var localPath = await _codeBuddyRepository.GetTempFolderPath();
                var response = await _codeBuddyRepository.GetWorkspaceAsync(ProjectId, localPath);
                var updateRepoStatus = await _codeBuddyRepository.UpdateWorkSpaceFileType(ProjectId);
                if (response.Count > 0)
                {
                    return new GetWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }

        }
        public async Task<GetWorkspaceDetailsByAuthTokenResponse> GetWorkspaceDetailsAsync(string authToken)
        {
            try
            {
                var response = await _codeBuddyRepository.GetWorkspaceDetailsAsync(authToken);
                if (response != null)
                {
                    return new GetWorkspaceDetailsByAuthTokenResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetWorkspaceDetailsByAuthTokenResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetWorkspaceDetailsByAuthTokenResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }

        }
        public async Task<GetAllProjectsResponse> GetAllProjectsAsync(string token)
        {
            try
            {
                var response = await _codeBuddyRepository.GetAllProjectsAsync(token);
                if (response.Count > 0)
                {
                    return new GetAllProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetAllProjectsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetAllProjectsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetModelsResponse> GetModelsAsync()
        {
            try
            {
                var response = await _codeBuddyRepository.GetModelsAsync();
                if (response.Count > 0)
                {
                    return new GetModelsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetModelsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetModelsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetWorkspaceDetailsResponse> GetWorkspaceDetailsAsync(int itemId, string itemType)
        {
            try
            {
                var response = await _codeBuddyRepository.GetWorkspaceDetailsAsync(itemId, itemType);
                if (response != null)
                {
                    return new GetWorkspaceDetailsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetWorkspaceDetailsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetWorkspaceDetailsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<SyncWorkspaceResponse> SyncWorkspace(SyncWorkspaceDTO request)
        {
            try
            {
                SyncWorkspaceResponse response;
                var mappingRequest = _mapper.Map<ImportFileDTO>(request);

                var project = await _codeBuddyRepository.GetProjectByRepoUrl(mappingRequest);
                if (project != null)
                {
                    mappingRequest.GitHubURL = project.GitHubURL;
                    mappingRequest.BranchName = project.BranchName;
                    mappingRequest.Password = project.Password;
                    if (project.IsInMemory)
                    {
                        var repo = await _codeBuddyRepository.IsDevopsRepoExist(mappingRequest);
                        if (repo.Status == 0)
                        {
                            var checkIfProjectisModified = await _codeBuddyRepository.CheckDevopsProjectCheckSumIsExist(mappingRequest.GitHubURL, project.Password, project.BranchName, repo.Item);
                            if (!checkIfProjectisModified)
                            {
                                var detectDevopsNewOrModifiedOrDeletedFiles = await _codeBuddyRepository.DetectDevopsNewOrUpdatedOrDeletedFiles(mappingRequest, repo.Item, project.Id);
                                if (detectDevopsNewOrModifiedOrDeletedFiles == true)
                                {
                                    await _codeBuddyRepository.UpdateDevopsProjectCheckSum(project.GitHubURL, project.Password, project.BranchName, repo.Item, project.Id);
                                    return new SyncWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                                }
                                else
                                {
                                    return new SyncWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorSync);
                                }
                            }
                            return new SyncWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.AlreadySync);
                        }
                        return new SyncWorkspaceResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
                    }
                    else
                    {
                        var localPath = await _codeBuddyRepository.GetTempFolderPath();


                        (bool isSuccess, bool isPrivate, bool isBranchExist, string message) cloneSuccess = (false, request.isPrivate, false, "");
                        if (request.isPrivate == false)
                            cloneSuccess = await _codeBuddyRepository.ClonePublicRepository(mappingRequest, localPath);
                        else if (request.isPrivate == true)
                            cloneSuccess = await _codeBuddyRepository.ClonePrivateRepository(mappingRequest, localPath);

                        if (cloneSuccess.isSuccess == true)
                        {
                            var folderName = await _codeBuddyRepository.GetRepositoryName(project.GitHubURL);
                            var checkIfProjectisModified = await _codeBuddyRepository.CheckProjectCheckSumIsExist(localPath, folderName);
                            if (!checkIfProjectisModified)
                            {
                                //getall files from devops repo
                                var fileDetailsList = await _codeBuddyRepository.GetAllFileDetails(project.Id, project.GitHubURL, localPath);
                                if (fileDetailsList.Count > 0)
                                {
                                    //var blobFilesList = await _blobStorageService.ListBlobAsync(folderName);
                                    var detectNewOrModifiedOrDeletedFiles = await _codeBuddyRepository.DetectNewOrUpdatedOrDeletedFiles(fileDetailsList, localPath, project.Id);
                                    if (detectNewOrModifiedOrDeletedFiles == true)
                                    {
                                        await _codeBuddyRepository.UpdateProjectCheckSum(localPath, folderName, project.Id);
                                        await _codeBuddyRepository.DeleteDirectory(localPath);
                                        return new SyncWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                                    }
                                    else
                                    {
                                        await _codeBuddyRepository.UpdateProjectCheckSum(localPath, folderName, project.Id);
                                        await _codeBuddyRepository.DeleteDirectory(localPath);
                                        return new SyncWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorSync);
                                    }
                                }
                                await _codeBuddyRepository.DeleteDirectory(localPath);
                                return new SyncWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorWorkspaceEmpty);
                            }
                            await _codeBuddyRepository.DeleteDirectory(localPath);
                            return new SyncWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.AlreadySync);
                        }
                        else if (cloneSuccess.isSuccess == false && cloneSuccess.isPrivate == true)
                        {
                            return new SyncWorkspaceResponse(StatusAndMessagesKeys.PrivateRepositoryError, StatusAndMessagesKeys.PrivateRepository);
                        }
                        else
                        {
                            return new SyncWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.Workspacecloningfailed);
                        }
                    }

                }
                else
                {
                    return new SyncWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAlreadyExist);
                }
            }
            catch (Exception ex)
            {
                return new SyncWorkspaceResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<DeleteWorkspaceResponse> DeleteWorkspace(int projectId)
        {
            try
            {
                var response = await _codeBuddyRepository.DeleteWorkspaceAsync(projectId);
                if (response != null)
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound);
                }
            }
            catch (Exception ex)
            {
                return new DeleteWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<EditWorkspaceResponse> EditWorksapce(EditWorkspaceDTO request)
        {
            try
            {
                var response = await _codeBuddyRepository.EditWorkspaceAsync(request);
                if (response)
                {
                    return new EditWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new EditWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new EditWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<InviteProjectResponse> InviteProjectAsync(InviteProjectDTO request)
        {
            try
            {
                var response = await _codeBuddyRepository.InviteProjectByEmail(request);
                if (response)
                {
                    return new InviteProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new InviteProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound);
                }
            }
            catch (Exception ex)
            {
                return new InviteProjectResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<InviteAcceptResponse> InviteAcceptAsync(string inviteToken, string authToken)
        {
            try
            {
                var response = await _codeBuddyRepository.InviteAcceptAsyncByURL(inviteToken, authToken);
                if (response)
                {
                    return new InviteAcceptResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new InviteAcceptResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound);
                }
            }
            catch (Exception ex)
            {
                return new InviteAcceptResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<bool> CodeCoppied(CodeSuggestionDTO codeSuggestionDTO)
        {
            try
            {
                var codeSuggestion = new CodeSuggestion
                {
                    NumberOfLinesCoppied = codeSuggestionDTO.NumberOfLinesOfCode,
                    ProjectId = codeSuggestionDTO.ProjectId,
                    CreatedDate = DateTime.UtcNow,
                    Query = codeSuggestionDTO.Prompt,
                    AuthToken = codeSuggestionDTO.AuthToken,
                    HistoryChatType = codeSuggestionDTO.ChatType,
                };
                var response = await _codeBuddyRepository.CodeCoppied(codeSuggestion);
                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<WorkSpaceStatsResponse> GetWorkSpaceStats(int? projectId, string? authToken)
        {
            try
            {
                var result = await _codeBuddyRepository.GetWorkSpaceStats(projectId, authToken);
                return result;
            }
            catch (Exception)
            {
                return new WorkSpaceStatsResponse
                {
                    ProjectId = projectId,
                    GraphEntriesResponse = null,
                };
            }
        }
        public async Task<GetBoilerPlatesTemplatesResponse> GetBoilerPlatesTemplates()
        {
            try
            {
                var response = await _codeBuddyRepository.GetBoilerPlatesTemplatesAsync();
                if (response.Count > 0)
                {
                    return new GetBoilerPlatesTemplatesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetBoilerPlatesTemplatesResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
                }
            }
            catch (Exception ex)
            {
                return new GetBoilerPlatesTemplatesResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<SubmitFeedbackResponse> SubmitFeedback(SubmitFeedbackDTO request)
        {
            try
            {
                var response = await _codeBuddyRepository.SubmitFeedbackAsync(request);
                if (response)
                {
                    return new SubmitFeedbackResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new SubmitFeedbackResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new SubmitFeedbackResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<GetUserSpecificExtensionAnalyticsResponse> GetUserSpecificExtensionAnalytics(string? authToken,int? historyChatType)
        {
            try
            {
                var response = await _codeBuddyRepository.GetUserSpecificExtensionAnalyticsAsync(authToken,historyChatType);
                if (response != null)
                {
                    return new GetUserSpecificExtensionAnalyticsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetUserSpecificExtensionAnalyticsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
                }
            }
            catch (Exception ex)
            {
                return new GetUserSpecificExtensionAnalyticsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetVisiBotStatsResponse> GetVisitBotStats(GetVisitBotStatsRequest request)
        {
            try
            {
                var result = await _codeBuddyRepository.GetVisiBotStats(request);
                if (result != null)
                {
                    return new GetVisiBotStatsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, result);
                }
                else
                {
                    return new GetVisiBotStatsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
                }
            }
            catch (Exception)
            {
                return new GetVisiBotStatsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<CreateCodeBuddyPerformanceResponse> CreateCodeBuddyPerformance(CreateCodeBuddyPerformanceRequest request)
        {
            try
            {
                var result = await _codeBuddyRepository.CreateCodeBuddyPerformanceAsync(request);
                if (result)
                {
                    return new CreateCodeBuddyPerformanceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new CreateCodeBuddyPerformanceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception)
            {
                return new CreateCodeBuddyPerformanceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }

        public async Task<(byte[], string)> GetAllIndexedFilesAsync(int projectId)
        {
            try
            {
                var result = await _codeBuddyRepository.GetAllIndexedFiles(projectId);


                if(result != null && result.Count > 0)
                {
                    var projectName = result.Select(x => x?.Project?.Name)
                        .FirstOrDefault();

                    var projectCreatedDate = result.Select(x => x?.Project?.CreateDate)
                        .FirstOrDefault();

                    var pdfBytes = GeneratePdf(projectName, projectCreatedDate.ToString(), result);

                    return (pdfBytes, projectName);

                }
            }
            catch (Exception ex)
            {
                return (null, string.Empty);
            }

            return (null, string.Empty);
        }



        private byte[] GeneratePdf(string projectName, string projectCreatedDate, List<CodeBuddyFileDetails> fileSummaries)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Title
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                Paragraph title = new Paragraph($"{projectName}\n{projectCreatedDate}", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph("\n| Code Analysis Report | Confidential\n", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));

                // Iterate through files and their summaries
                foreach (var file in fileSummaries)
                {
                    document.Add(new Paragraph($"\nFILENAME: {file.Name}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLUE)));

                    if (!string.IsNullOrEmpty(file.Summary))
                    {
                        var summaryLines = file.Summary.Split('\n').ToList();
                        iTextSharp.text.List list = new iTextSharp.text.List(iTextSharp.text.List.UNORDERED);
                        list.SetListSymbol("\u2022"); // Bullet point
                        foreach (var line in summaryLines)
                        {
                            //list.Add(new iTextSharp.text.ListItem(line, FontFactory.GetFont(FontFactory.HELVETICA, 12)));

                            if (line.Trim().StartsWith("####")) // Check if it's a section title
                            {
                                string titleText = line.Replace("####", "").Trim(); // Remove "####"
                                Paragraph subTitle = new Paragraph(titleText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, BaseColor.BLACK));
                                list.Add(new iTextSharp.text.ListItem(titleText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, BaseColor.BLACK)));
                                //document.Add(subTitle); // Add as a title
                            }
                            else
                            {
                                //list.Add(new iTextSharp.text.ListItem(line, FontFactory.GetFont(FontFactory.HELVETICA, 12)));


                                if (line.Contains("**")) {
                                    string newline = Regex.Replace(line, @"\*\*", "");
                                    list.Add(new iTextSharp.text.ListItem(line, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                                }

                                //if (line.Contains("**"))
                                //{
                                //    var parts = line.Split("**"); // Split text by "**"
                                //    Phrase formattedText = new Phrase();

                                //    for (int i = 0; i < parts.Length; i++)
                                //    {
                                //        if (i % 2 == 1) // Bold Text (inside **)
                                //        {
                                            
                                //            formattedText.Append(new Chunk(parts[i], FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                                //        }
                                //        else // Normal Text
                                //        {
                                //            formattedText.Append(new Chunk(parts[i], FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                                //        }
                                //    }

                                //    list.Add(new iTextSharp.text.ListItem(formattedText));
                                //}
                                //else
                                //{
                                //    list.Add(new iTextSharp.text.ListItem(line, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                                //}
                            }

                        }
                        document.Add(list);
                    }
                    else
                    {
                        document.Add(new Paragraph("No security, auditing, or regulatory details found.", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                    }
                }

                document.Close();
                return stream.ToArray();
            }
        }


    }
}






