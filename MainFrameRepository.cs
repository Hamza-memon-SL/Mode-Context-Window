using GenAiPoc.Contracts.Context;
using GenAiPoc.Contracts.Models;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using GenAiPoc.Infrastructure.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAct;

namespace GenAiPoc.Services
{

    public class MainFrameRepository : IMainFrameRepository
    {
        private readonly DbContextGenAiPOC _dbContextGenAiPOC;
        private readonly IBlobStorageService _blobStorageService;

        public MainFrameRepository(DbContextGenAiPOC dbContextGenAiPOC, IBlobStorageService blobStorageService)
        {
            this._dbContextGenAiPOC = dbContextGenAiPOC;
            _blobStorageService = blobStorageService;
        }

        #region Public Methods

        public async Task<MainFrameSourceProject?> GetMainFrameSourceProjectByRepoUrlOrByFile(ImportMainFrameProjectDTO model, List<IFormFile> files)
        {
            MainFrameSourceProject? project = null;
            if (model.ProjectId == 0)
            {
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fileName = file.FileName;
                        string fileExtension = Path.GetExtension(fileName).ToLower();

                        if (fileExtension == ".zip")
                        {
                            var zipFileName = Path.GetFileNameWithoutExtension(fileName);
                            project = await _dbContextGenAiPOC.MainFrameSourceProject
                                .Where(x => x.ZipFile.Equals(fileName) && x.AuthToken.Equals(model.Token))
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
                        }
                        else
                        {
                            project = await _dbContextGenAiPOC.MainFrameSourceProject
                                .Where(x => x.Name.Equals(model.Name) && x.AuthToken.Equals(model.Token))
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
                        }
                    }
                }
                else
                {
                    project = await _dbContextGenAiPOC.MainFrameSourceProject
                        .Where(x => x.ImportURL.Equals(model.ImportURL)
                        && x.AuthToken.Equals(model.Token)).AsNoTracking()
                        .FirstOrDefaultAsync();
                }
            }
            else
            {
                project = await _dbContextGenAiPOC.MainFrameSourceProject
                    .Where(x => x.Id.Equals(model.ProjectId))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            return project;
        }
        public async Task<string> GetTempMainFrameSourceFolderPath()
        {
            try
            {

                var basePath = @"C:\home";
                var tempFolderPath = Path.Combine(basePath, "MainFrameSourceTempFolder");
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                return tempFolderPath;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                throw;
            }

        }
        public async Task<string> GetTempMainFrameDestinationFolderPath()
        {
            try
            {

                var basePath = @"C:\home";
                var tempFolderPath = Path.Combine(basePath, "MainFrameDestinationTempFolder");
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                return tempFolderPath;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                throw;
            }

        }
        public async Task<(bool isSuccess, bool isPrivate, bool isBranchExist, string message)> ClonePublicMainFrameSourceRepository(ImportMainFrameProjectDTO dto, string localPath)
        {
            bool isSuccess = false;

            try
            {
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                var repoName = await GetRepositoryName(dto.ImportURL);
                var repoPath = Path.Combine(localPath, repoName);

                var cloneOptions = new CloneOptions
                {
                    BranchName = dto.BranchName
                };

                Repository.Clone(dto.ImportURL, repoPath, cloneOptions);

                Console.WriteLine($"Repository cloned successfully into {repoPath}!");

                return (isSuccess = true, dto.isPrivate = false, true, "Successfull");
            }
            catch (LibGit2Sharp.NotFoundException ex)
            {
                return (isSuccess = false, dto.isPrivate = false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cloning repository: " + ex.Message);

                if (ex.Message.Contains("401") || ex.Message.Contains("remote authentication required"))
                {
                    return (isSuccess = false, dto.isPrivate = true, true, "remote authentication required" + ex.Message);
                }
                return (isSuccess = false, dto.isPrivate = false, true, ex.Message);
            }

        }
        public async Task<ImportResponse> ProcessMainFrameSourceProjectByURLAsync(ImportMainFrameProjectDTO dto, string localPath)
        {
            try
            {
                var repoName = await GetRepositoryName(dto.ImportURL);
                var repoPath = Path.Combine(localPath, repoName);
                var sourceProjectAdded = await ImportMainFrameSourceProjectAsync(dto, repoPath, null, null);
                if (sourceProjectAdded.Status == 0)
                {
                    var project = await GetMainFrameSourceProjectByRepoUrlOrByFile(dto, null);
                    if (project != null)
                    {
                        Console.WriteLine("Repository Cloned Successfully");
                        var destinationProjectAdded = await ImportMainFrameDestinationProjectAsync(dto, project.Id);
                        var fileDetailsList = await GetAllFileDetails(project.Id, project.ImportURL, localPath, dto);
                        var filepaths = fileDetailsList.Select(f => f.FullPath).ToList();
                        Console.WriteLine(filepaths);
                        if (fileDetailsList.Any())
                        {
                            var filesUploaded = await _blobStorageService.UploadMainFrameSourceProjectByURLToBlob(fileDetailsList, project, localPath);
                            if (filesUploaded.Count > 0)
                            {
                                bool filesSaved = await SaveMainFrameSourceFileDetailsToDatabase(fileDetailsList);
                                if (filesSaved)
                                {
                                    Console.WriteLine("File Details Added Successfully");
                                    await DeleteDirectory(localPath);
                                }

                                Console.WriteLine("Repository cloned and uploaded successfully.");

                            }
                        }
                        return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAddedSuccessfully);

                    }
                    else
                    {
                        return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound);
                    }

                }
                else
                {
                    Console.WriteLine("Project Added Failed");
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAddedFailed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing repository: {ex.Message}");
                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }

        }
        public async Task<ImportResponse> ProcessMainFrameSourceProjectByZipOrFileAsync(ImportMainFrameProjectDTO dto, List<IFormFile> files)
        {
            try
            {


                //// Create a folder on Blob with ZIP name
                //string zipFolderName = Path.GetFileNameWithoutExtension(zipFile.FileName);

                var sourceProjectAdded = await ImportMainFrameSourceProjectAsync(dto, null, null, files);
                if (sourceProjectAdded.Status == 0)
                {
                    var project = await GetMainFrameSourceProjectByRepoUrlOrByFile(dto, files);
                    if (project != null)
                    {
                        Console.WriteLine("Repository Cloned Successfully");
                        var destinationProjectAdded = await ImportMainFrameDestinationProjectAsync(dto, project.Id);
                        var filesUploadedList = await _blobStorageService.UploadMainFrameSourceProjectZipOrFileToBlob(files, project);
                        if (filesUploadedList.Count > 0)
                        {
                            bool filesSaved = await SaveMainFrameSourceFileDetailsToDatabase(filesUploadedList);

                            Console.WriteLine("Repository cloned and uploaded successfully.");
                        }
                    }
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAddedSuccessfully);
                }
                else
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectAddedFailed);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing repository: {ex.Message}");
                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }

        }
        public bool HasValidZipOrFiles(List<IFormFile> files)
        {
            if (files == null || !files.Any()) return false;

            // Check if there's a ZIP file
            var zipFile = files.FirstOrDefault(f => Path.GetExtension(f.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase));

            if (zipFile != null)
            {
                // Process ZIP file
                using (var zipStream = zipFile.OpenReadStream())
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    return archive.Entries.Any(e => !string.IsNullOrWhiteSpace(e.Name)); // ZIP contains at least one valid file
                }
            }

            // If no ZIP, check if at least one non-empty file exists
            return files.Any(f => f.Length > 0);
        }
        public async Task<List<MainFrameSourceProject>> GetMainFrameProjectDetailsByAuthTokenAsync(string authToken)
        {
            try
            {

                var project = await _dbContextGenAiPOC.MainFrameSourceProject
                .Where(project => project.AuthToken.Equals(authToken))
                   .ToListAsync();
                return project;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> EditMainFrameSourceProjectAsync(EditMainFrameSourceProjectDTO request)
        {
            bool isSuccess = false;
            try
            {
                var projectObj = await _dbContextGenAiPOC.MainFrameSourceProject
                    .Where(x => x.Id.Equals(request.projectId)).AsNoTracking()
                    .FirstOrDefaultAsync();

                if (projectObj != null)
                {
                    projectObj.Name = request.projectName;
                    projectObj.Description = request.projectDescription;

                    _dbContextGenAiPOC.MainFrameSourceProject.Update(projectObj);
                    int isUpdated = await _dbContextGenAiPOC.SaveChangesAsync();
                    if (isUpdated > 0)
                    {
                        return isSuccess = true;
                    }

                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                return isSuccess = false;
            }
        }
        public async Task<bool> DeleteMainFrameSourceProjectServiceAsync(int projectId)
        {
            bool isSuccess = false;
            try
            {
                var project = await _dbContextGenAiPOC.MainFrameSourceProject
                    .Include(x => x.MainFrameSourceFiles)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project != null)
                {
                    //var folderName = await GetRepositoryName(project.ImportURL);
                    string basePath = !string.IsNullOrEmpty(project.ImportURL)
                                     ? $"{project.CreatedBy}/Library"
                                     : !string.IsNullOrEmpty(project.ZipFile)
                                         ? $"{project.CreatedBy}/ZIP"
                                         : $"{project.CreatedBy}/Files";
                    string folderName = !string.IsNullOrEmpty(project.ImportURL)
                                          ? await GetRepositoryName(project.ImportURL)
                                          : !string.IsNullOrEmpty(project.ZipFile)
                                              ? project.ZipFile
                                              : project.Name;

                    var deleteBlob = await _blobStorageService.DeleteFolderAsync(basePath, folderName);
                    if (deleteBlob)
                    {
                        var destinationFiles = await _dbContextGenAiPOC.MainFrameDestinationFiles
                                 .Where(df => df.MainFrameDestinationProject.MainFrameSourceProjectId == projectId)
                                 .ToListAsync();

                        var destinationProjects = await _dbContextGenAiPOC.MainFrameDestinationProject
                            .Where(dp => dp.MainFrameSourceProjectId == projectId)
                            .ToListAsync();

                        _dbContextGenAiPOC.MainFrameDestinationFiles.RemoveRange(destinationFiles);
                        _dbContextGenAiPOC.MainFrameDestinationProject.RemoveRange(destinationProjects);
                        _dbContextGenAiPOC.MainFrameSourceFiles.RemoveRange(project.MainFrameSourceFiles);
                        _dbContextGenAiPOC.MainFrameSourceProject.Remove(project);
                        var isDeleted = await _dbContextGenAiPOC.SaveChangesAsync();
                        if (isDeleted > 0)
                        {
                            return isSuccess = true;
                        }
                    }
                    return isSuccess = false;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                return isSuccess = false;
            }
        }
        public async Task<GetMainFrameProjectDetails> GetMainFrameProjectDetailsAsync(int itemId, string itemType, string projectType)
        {
            try
            {
                GetMainFrameProjectDetails projectDetails;
                if (projectType == "Source")
                {
                    if (itemType == "Project")
                    {
                        projectDetails = await _dbContextGenAiPOC.MainFrameSourceProject.Where(x => x.Id.Equals(itemId))
                               .Select(x => new GetMainFrameProjectDetails
                               {
                                   ItemId = x.Id,
                                   ItemType = "Project",
                                   Content = "",
                                   Summary = x.Summary,
                               }).FirstOrDefaultAsync();
                    }
                    else
                    {
                        var fileDetail = await _dbContextGenAiPOC.MainFrameSourceFiles
                                    .Where(x => x.Id.Equals(itemId))
                                      .FirstOrDefaultAsync();

                        if (fileDetail != null)
                        {
                            projectDetails = new GetMainFrameProjectDetails
                            {
                                ItemId = fileDetail.Id,
                                ItemType = "File",
                                Content = await _blobStorageService.GetBlobContentAsync(fileDetail.FullPath),
                                Summary = fileDetail?.Summary,
                            };

                            return projectDetails;
                        }

                        return null;
                    }
                }
                else
                {
                    if (itemType == "Project")
                    {
                        projectDetails = await _dbContextGenAiPOC.MainFrameDestinationProject.Where(x => x.Id.Equals(itemId))
                               .Select(x => new GetMainFrameProjectDetails
                               {
                                   ItemId = x.Id,
                                   ItemType = "Project",
                                   Content = "",
                                   //Summary = x.Summary,
                               }).FirstOrDefaultAsync();
                    }
                    else
                    {
                        var fileDetail = await _dbContextGenAiPOC.MainFrameDestinationFiles
                                    .Where(x => x.Id.Equals(itemId))
                                      .FirstOrDefaultAsync();

                        if (fileDetail != null)
                        {
                            projectDetails = new GetMainFrameProjectDetails
                            {
                                ItemId = fileDetail.Id,
                                ItemType = "File",
                                Content = await _blobStorageService.GetBlobContentAsync(fileDetail.FullPath),
                                Summary = fileDetail?.Summary,
                            };

                            return projectDetails;
                        }

                        return null;
                    }
                }

                return projectDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<MainFrameSourceProjectTree>> GetMainFrameSourceProjectAsync(int projectId)
        {
            try
            {
                var project = await _dbContextGenAiPOC.MainFrameSourceProject
                    .Include(p => p.MainFrameSourceFiles)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null)
                {
                    return new List<MainFrameSourceProjectTree>();
                }

                var fileDetailsList = await _dbContextGenAiPOC.MainFrameSourceFiles
                            .Where(x => x.MainFrameSourceProjectId.Equals(projectId))
                              .ToListAsync();
                if (fileDetailsList == null || !fileDetailsList.Any())
                {
                    return new List<MainFrameSourceProjectTree>();
                }

                var projectTree = new MainFrameSourceProjectTree
                {
                    Id = projectId.ToString(),
                    Name = $"{project.Name}",
                    Type = "project",
                    Children = new List<MainFrameSourceProjectTree>()
                };

                var rootDirectoryName = GetSourceProjectRootDirectoryName(project.MainFrameSourceFiles.ToList());
                BuildSourceProjectWorkspaceTree(projectTree, project.MainFrameSourceFiles.ToList(), rootDirectoryName, projectId);

                return new List<MainFrameSourceProjectTree> { projectTree };
            }
            catch (Exception ex)
            {
                return new List<MainFrameSourceProjectTree>();
            }
        }
        public async Task<List<MainFrameSourceProjectTree>> GetMainFrameDestinationProjectAsync(int projectId)
        {
            try
            {
                var project = await _dbContextGenAiPOC.MainFrameDestinationProject
                    .Include(p => p.MainFrameDestinationFiles)
                     .Include(p => p.MainFrameSourceProject)
                    .FirstOrDefaultAsync(p => p.MainFrameSourceProjectId == projectId);

                if (project == null)
                {
                    return new List<MainFrameSourceProjectTree>();
                }

                var fileDetailsList = await _dbContextGenAiPOC.MainFrameDestinationFiles
                            .Where(x => x.MainFrameDestinationProject.Id.Equals(project.Id))
                              .ToListAsync();
                //string projectName = fileDetailsList == null || !fileDetailsList.Any()
                //                      ? project.MainFrameSourceProject?.Name ?? project.MainFrameSourceProject?.Name
                //                      : project.Name;

                string projectName = (fileDetailsList == null || !fileDetailsList.Any())
                                        ? (!string.IsNullOrEmpty(project.MainFrameSourceProject?.ImportURL)
                                            ? await GetRepositoryName(project.MainFrameSourceProject.ImportURL)
                                            : (!string.IsNullOrEmpty(project.FullPath)
                                                ? await GetRepositoryName(project.FullPath)
                                                : project?.Name))
                                        : await GetRepositoryName(project.FullPath);


                //if (fileDetailsList == null || !fileDetailsList.Any())
                //{
                //    return new List<MainFrameSourceProjectTree>();
                //}

                var projectTree = new MainFrameSourceProjectTree
                {
                    Id = projectId.ToString(),
                    Name = $"{projectName}",
                    Type = "project",
                    Children = new List<MainFrameSourceProjectTree>()
                };

                if (fileDetailsList.Any())
                {
                    var rootDirectoryName = GetDestinationProjectRootDirectoryName(fileDetailsList);
                    BuildDestinationProjectWorkspaceTree(projectTree, fileDetailsList, rootDirectoryName, projectId);
                }

                return new List<MainFrameSourceProjectTree> { projectTree };
            }
            catch (Exception ex)
            {
                return new List<MainFrameSourceProjectTree>();
            }
        }
        public async Task<MainFrameDestinationProject?> GetMainFrameDestinationProjectByIdAsync(CreateMainFrameChunkFileDTO request)
        {
            MainFrameDestinationProject? project = null;
            project = await _dbContextGenAiPOC.MainFrameDestinationProject
                .Where(x => x.MainFrameSourceProjectId.Equals(request.MainFrameSourceProjectId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return project;
        }
        public async Task<MainFrameDestinationProject?> GetMainFrameDestinationProjectByIdAsync(int destinationProjectId)
        {
            MainFrameDestinationProject? project = null;
            project = await _dbContextGenAiPOC.MainFrameDestinationProject
                .Where(x => x.Id.Equals(destinationProjectId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return project;
        }
        public async Task<MainFrameDestinationFiles?> GetMainFrameDestinationProjectFileByIdAsync(int destinationFileId)
        {
            MainFrameDestinationFiles? file = null;
            file = await _dbContextGenAiPOC.MainFrameDestinationFiles
                .Where(x => x.Id.Equals(destinationFileId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return file;
        }
        //public async Task<bool> ProcessMainFrameDestinationProjectAsync(CreateMainFrameChunkFileDTO request, MainFrameDestinationProject mainFrameDestinationProject)
        //{
        //    bool isSuccess = false;
        //    try
        //    {
        //        var mainFrameDestinationFile = await GetMainFrameDestionationProjectAllFileDetails(request);

        //        if (mainFrameDestinationFile != null)
        //        {
        //            var uploadMainFrameDestinationFileToBlob = await _blobStorageService.UploadMainFrameDestinationProjectByFileToBlob(mainFrameDestinationFile, request);
        //            if (uploadMainFrameDestinationFileToBlob != null)
        //            {
        //                bool filesSaved = await SaveMainFrameDestinationFileDetailsToDatabase(mainFrameDestinationFile);
        //                if (filesSaved && !string.IsNullOrEmpty(mainFrameDestinationProject.FullPath))
        //                {
        //                    var updateDestinationProject = await UpdateMainFrameDestinationProjectAsync(mainFrameDestinationFile);
        //                    isSuccess = true;
        //                }
        //                if (filesSaved)
        //                {
        //                    isSuccess = true;
        //                }
        //            }
        //            else
        //            {
        //                isSuccess = false;
        //            }
        //        }
        //        else
        //        {
        //            isSuccess = false;
        //        }
        //        return isSuccess;

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error processing repository: {ex.Message}");
        //        return isSuccess;
        //    }

        //}
        public async Task<MainFrameDestinationFiles> GetMainFrameDestionationProjectAllFileDetails(CreateMainFrameChunkFileDTO request, MainFrameDestinationProject mainFrameDestinationProject)
        {
            try
            {
                if (request.MainFrameDestinationFile != null)
                {
                    var mainFrameDestinationFile = new MainFrameDestinationFiles
                    {
                        ChunkFileName = request?.MainFrameDestinationFileName,
                        FullPath = request.MainFrameDestinationFilePath,
                        Extension = Path.GetExtension(request?.MainFrameDestinationFile.FileName),
                        Size = FormatFileSize(request.MainFrameDestinationFile.Length),
                        //MainFrameSourceFiles = new MainFrameSourceFiles
                        //{
                        //    Id = request.MainFrameSourceProjectFileId,
                        //},
                        MainFrameSourceFilesId = request.MainFrameSourceFileId,
                        MainFrameDestinationProjectId = mainFrameDestinationProject.Id,
                        StartLine = request.MainFrameDestinationFileStartLine ?? 0,
                        EndLine = request.MainFrameDestinationFileEndLine ?? 0,
                        ColorCode = request.MainFrameDestinationFileColorCode,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = true,
                        AuthToken = request?.AuthToken,
                        CreatedBy = request?.CreatedBy,
                        Status = SummarizeMainFrame.New
                    };

                    return mainFrameDestinationFile;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Project Files Listing Failed due to this error: " + ex.Message);
                throw;
            }
        }
        public async Task<bool> SaveMainFrameDestinationFileDetailsToDatabase(MainFrameDestinationFiles file)
        {
            bool isSuccess = false;
            try
            {
                Console.WriteLine("Project Files Saving Function Started");

                _dbContextGenAiPOC.MainFrameDestinationFiles.AddAsync(file);
                int isFileDetailsSaved = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isFileDetailsSaved > 0)
                    return isSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Project Files Saving Function Failed");
                return isSuccess;
            }
            return isSuccess;
        }
        public async Task<bool> UpdateMainFrameDestinationProjectAsync(MainFrameDestinationFiles mainFrameDestinationFiles)
        {

            bool isSuccess = false;

            try
            {
                var mainFrameDestinationProject = await _dbContextGenAiPOC.MainFrameDestinationProject.FirstOrDefaultAsync(x => x.Id.Equals(mainFrameDestinationFiles.MainFrameDestinationProjectId));
                if (mainFrameDestinationProject != null)
                {
                    mainFrameDestinationProject.FullPath = await GetMainFrameDestinationProjectFullPath(mainFrameDestinationFiles.FullPath);
                }
                int isProjectUpdated = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isProjectUpdated > 0)
                {
                    isSuccess = true;
                }
                else
                {

                    isSuccess = false;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return isSuccess;
            }
        }
        public async Task<bool> UpdateMainFrameDestinationProjectFileAsync(MainFrameDestinationFiles mainFrameDestinationFile, string destinationFilePath)
        {
            bool isSuccess = false;
            try
            {
                mainFrameDestinationFile.FullPath = destinationFilePath;
                _dbContextGenAiPOC.MainFrameDestinationFiles.Update(mainFrameDestinationFile);
                int isFileUpdated = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isFileUpdated > 0)
                {
                    isSuccess = true;
                }
                else
                {

                    isSuccess = false;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return isSuccess;
            }
        }

        public async Task<bool> IsExportRepositoryPublic(string repoUrl)
        {
            string apiUrl = ConvertToApiUrl(repoUrl);

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                    return false;

                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic repoInfo = JsonConvert.DeserializeObject(jsonResponse);

                return repoInfo.@private == false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Repository> ExportCloneMainFrameRepositoryToLocalDirectory(ExportMainFrameProjectByURLServiceDTO dto, string localPath)
        {
            try
            {

                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                var repoName = await GetRepositoryName(dto.RepoURL);
                var repoPath = Path.Combine(localPath, repoName);
                string tempPath = Repository.Init(localPath, false);
                var cloneOptions = new CloneOptions();
                //var checkIsprivate = await IsExportRepositoryPublic(dto.RepoURL);
                //if (!checkIsprivate)
                //{ ////Check if the specified branch exists in the remote repo

                //}
                //else
                //{
                //    if (dto.Platform == PlatformType.GitHub)
                //    {
                //        cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                //        {
                //            Username = dto.RepoPAT,
                //            Password = ""
                //        };
                //        cloneOptions.BranchName = dto.RepoBranch;
                //    }
                //    Repository.Clone(dto.RepoURL, repoPath, cloneOptions);
                //    if (!DoesBranchExist(dto))
                //    {
                //        await InitializeBranchInRepo(dto, repoPath);
                //    }
                //}

                if (!DoesBranchExist(dto))
                {
                    await InitializeBranchInRepo(dto, repoPath);
                }
                else
                {
                    if (dto.Platform == PlatformType.GitHub || dto.Platform == PlatformType.GitLab)
                    {
                        cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = dto.RepoPAT,
                            Password = ""
                        };
                        cloneOptions.BranchName = dto.RepoBranch;
                    }
                    else if (dto.Platform == PlatformType.AzureDevops)
                    {
                        cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = dto.RepoUserName,
                            Password = dto.RepoPAT
                        };
                        cloneOptions.BranchName = dto.RepoBranch;
                    }
                    else if (dto.Platform == PlatformType.GitLab)
                    {
                        cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = dto.RepoUserName,
                            Password = dto.RepoPAT
                        };
                        cloneOptions.BranchName = dto.RepoBranch;
                    }
                    Repository.Clone(dto.RepoURL, repoPath, cloneOptions);
                }

                return new Repository(repoPath);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public async Task<Branch> GetOrCreateBranch(Repository repo, string branchName)
        {
            Branch branch = repo.Branches[branchName] ?? repo.CreateBranch(branchName);
            Commands.Checkout(repo, branch);
            return branch;
        }
        public async Task DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public async Task<bool> DeleteMainFrameDestinationFilesServiceAsync(List<int> filesId)
        {
            bool isSuccess = false;
            try
            {
                var projectFiles = await _dbContextGenAiPOC.MainFrameDestinationFiles.Where(x => filesId.Contains(x.Id)).ToListAsync();
                if (projectFiles.Any())
                {
                    foreach (var file in projectFiles)
                    {
                        var deleteBlob = await _blobStorageService.DeleteMainFrameDestinationBlobFile(file.FullPath);
                    }
                    _dbContextGenAiPOC.MainFrameDestinationFiles.RemoveRange(projectFiles);

                    var isDeleted = await _dbContextGenAiPOC.SaveChangesAsync();
                    if (isDeleted > 0)
                    {
                        return isSuccess = true;
                    }
                }
                return isSuccess = false;
            }
            catch (Exception ex)
            {
                return isSuccess = false;
            }
        }
        #endregion

        #region Private Methods

        private async Task<string> GetRepositoryName(string repoUrl)
        {
            if (string.IsNullOrWhiteSpace(repoUrl))
                return string.Empty;

            var uri = new Uri(repoUrl);
            var segments = uri.Segments;
            var lastSegment = Uri.UnescapeDataString(segments[segments.Length - 1]);

            var repoName = Path.GetFileNameWithoutExtension(lastSegment);

            return repoName;
        }
        private async Task<ImportResponse> ImportMainFrameSourceProjectAsync(ImportMainFrameProjectDTO dto, string repoPath, string repositoryId, List<IFormFile>? files)
        {

            bool isSuccess = false;

            try
            {
                var checksum = string.Empty;
                if (!string.IsNullOrEmpty(repoPath))
                {

                    checksum = GetRepositoryChecksum(repoPath);
                }
                //if (dto.IsInMemory)
                //{
                //    checksum = await GetDevOpsChecksum(dto.GitHubURL, dto.Password, dto.BranchName, repositoryId);
                //}
                var mainFrameSourceProject = new MainFrameSourceProject
                {
                    ImportURL = dto.ImportURL,
                    Name = dto.Name,
                    BranchName = dto.BranchName,
                    Description = dto.Description,
                    Summary = null,
                    CreatedBy = dto.Email,
                    AuthToken = dto.Token,
                    CreatedDate = DateTime.Now,
                    ProjectChecksum = checksum,
                    IsActive = true,
                    ModifiedDate = DateTime.Now,
                    SourceType = GetSourceType(dto.ImportURL, files, out string zipFileName),
                    ZipFile = !string.IsNullOrEmpty(zipFileName) ? zipFileName : null,
                    IsPrivate = dto.isPrivate,
                    ProjectType = GetProjectType(dto.ImportURL),
                };

                await _dbContextGenAiPOC.MainFrameSourceProject.AddAsync(mainFrameSourceProject);
                int isProjectAdded = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isProjectAdded > 0)
                {
                    isSuccess = true;
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAddedSuccessfully);
                }
                else
                {

                    isSuccess = false;
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAlreadyExist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        private async Task<ImportResponse> ImportMainFrameDestinationProjectAsync(ImportMainFrameProjectDTO dto, int sourceProjectId)
        {

            bool isSuccess = false;

            try
            {
                var mainFrameDestinationProject = new MainFrameDestinationProject
                {
                    MainFrameSourceProjectId = sourceProjectId,
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedBy = dto.Email,
                    AuthToken = dto.Token,
                    CreatedDate = DateTime.Now,
                    Status = SummarizeMainFrame.New,
                    IsActive = true
                };

                await _dbContextGenAiPOC.MainFrameDestinationProject.AddAsync(mainFrameDestinationProject);
                int isProjectAdded = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isProjectAdded > 0)
                {
                    isSuccess = true;
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAddedSuccessfully);
                }
                else
                {

                    isSuccess = false;
                    return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectAlreadyExist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        private async Task<bool> SaveMainFrameSourceFileDetailsToDatabase(List<MainFrameSourceFiles> fileDetailsList)
        {
            bool isSuccess = false;
            try
            {
                Console.WriteLine("Project Files Saving Function Started");

                _dbContextGenAiPOC.MainFrameSourceFiles.AddRange(fileDetailsList);
                int isFileDetailsSaved = await _dbContextGenAiPOC.SaveChangesAsync();

                if (isFileDetailsSaved > 0)
                    return isSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Project Files Saving Function Failed");
                return isSuccess;
            }
            return isSuccess;
        }
        public async Task<List<MainFrameSourceFiles>> GetAllFileDetails(int projectId, string repoUrl, string localPath, ImportMainFrameProjectDTO dto)
        {
            try
            {
                var repoName = await GetRepositoryName(repoUrl);
                var repoPath = Path.Combine(localPath, repoName);

                Console.WriteLine("Project Files Repo Name: " + repoName);


                var fileDetailsList = new List<MainFrameSourceFiles>();

                using (var repo = new Repository(repoPath))
                {
                    var commit = repo.Head.Tip;
                    TraverseTree(commit.Tree, repoPath, projectId, fileDetailsList, dto);
                }
                Console.WriteLine("Project Files Listed and Tokens Calculated Successfully");
                return fileDetailsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Project Files Listing Failed due to this error: " + ex.Message);
                throw;
            }
        }
        private string GetRepositoryChecksum(string repoPath)
        {
            using (var repo = new Repository(repoPath))
            {
                var commit = repo.Head.Tip;
                return commit.Id.Sha;
            }
        }
        private string GetProjectType(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "Unknown or Self-Hosted or ZIP";
            }

            if (url.Contains("github.com", StringComparison.OrdinalIgnoreCase))
            {
                return "GitHub";
            }
            else if (url.Contains("gitlab.com", StringComparison.OrdinalIgnoreCase) || url.Contains("gitlab", StringComparison.OrdinalIgnoreCase))
            {
                return "GitLab";
            }
            else if (url.Contains("bitbucket.org", StringComparison.OrdinalIgnoreCase))
            {
                return "Bitbucket";
            }
            else if (url.Contains("dev.azure.com", StringComparison.OrdinalIgnoreCase) || url.Contains(".visualstudio.com", StringComparison.OrdinalIgnoreCase))
            {
                return "Azure DevOps";
            }
            else
            {
                return "Unknown or Self-Hosted or ZIP";
            }
        }
        private void TraverseTree(Tree tree, string repoPath, int projectId, List<MainFrameSourceFiles> fileDetailsList, ImportMainFrameProjectDTO dto)
        {
            foreach (var entry in tree)
            {
                var entryPath = Path.Combine(repoPath, entry.Name);

                if (entry.TargetType == TreeEntryTargetType.Blob)
                {
                    ProcessBlobEntry(entry, entryPath, projectId, fileDetailsList, dto);
                }
                else if (entry.TargetType == TreeEntryTargetType.Tree)
                {
                    // If entry is a Tree (subdirectory), recursively traverse it
                    var subTree = (Tree)entry.Target;
                    TraverseTree(subTree, entryPath, projectId, fileDetailsList, dto);
                }
            }
        }
        private void ProcessBlobEntry(TreeEntry entry, string filePath, int projectId, List<MainFrameSourceFiles> fileDetailsList, ImportMainFrameProjectDTO dto)
        {
            var blob = (Blob)entry.Target;

            if (File.Exists(filePath) && IsValidFile(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                fileDetailsList.Add(new MainFrameSourceFiles
                {
                    Name = fileInfo.Name,
                    FullPath = fileInfo.FullName,
                    Extension = fileInfo.Extension,
                    Size = FormatFileSize(fileInfo.Length),
                    CreatedDate = fileInfo.CreationTime,
                    MainFrameSourceProjectId = projectId,
                    FileCheckSum = blob.Id.Sha,
                    LineCount = File.ReadAllLines(filePath).Length,
                    ModifiedDate = fileInfo.CreationTime,
                    IsActive = true,
                    AuthToken = dto?.Token,
                    CreatedBy = dto?.Email
                    //FileType = (int?)EnumFileType.Initial,
                });
            }
        }
        private bool IsValidFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return !fileInfo.Name.Equals(".config", StringComparison.OrdinalIgnoreCase) &&
                   (fileInfo.Attributes & FileAttributes.Hidden) == 0 &&
                   (fileInfo.Directory.Attributes & FileAttributes.Hidden) == 0 &&
                   !IsUnderGitDirectory(fileInfo.FullName); // Skip .git directories
        }
        private static string FormatFileSize(long bytes)
        {
            return $"{bytes} bytes";
        }
        private bool IsUnderGitDirectory(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            while (directoryName != null && directoryName != filePath)
            {
                if (Path.GetFileName(directoryName).Equals(".git", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                directoryName = Path.GetDirectoryName(directoryName);
            }
            return false;
        }

        private ProjectSourceType GetSourceType(string importUrl, List<IFormFile> files, out string zipFileName)
        {
            zipFileName = "";

            if (!string.IsNullOrWhiteSpace(importUrl))
            {
                return ProjectSourceType.Library;
            }

            var zipFile = files?.FirstOrDefault(f => Path.GetExtension(f.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase));
            if (zipFile != null)
            {
                zipFileName = zipFile.FileName;
                return ProjectSourceType.ZipUpload;
            }
            return ProjectSourceType.FilesUpload;
        }
        private string GetSourceProjectRootDirectoryName(List<MainFrameSourceFiles> files)
        {
            if (files == null || !files.Any())
                return string.Empty;
            var commonPath = Path.GetDirectoryName(files.First().FullPath);
            foreach (var file in files)
            {
                var filePath = Path.GetDirectoryName(file.FullPath);
                while (!filePath.StartsWith(commonPath))
                {
                    commonPath = Path.GetDirectoryName(commonPath);
                }
            }
            return commonPath;
        }
        private string GetDestinationProjectRootDirectoryName(List<MainFrameDestinationFiles> files)
        {
            if (files == null || !files.Any())
                return string.Empty;
            var commonPath = Path.GetDirectoryName(files.First().FullPath);
            foreach (var file in files)
            {
                var filePath = Path.GetDirectoryName(file.FullPath);
                while (!filePath.StartsWith(commonPath))
                {
                    commonPath = Path.GetDirectoryName(commonPath);
                }
            }
            return commonPath;
        }
        private void BuildSourceProjectWorkspaceTree(MainFrameSourceProjectTree rootNode, List<MainFrameSourceFiles> files, string rootDirectoryName, int projectId)
        {
            var folderNodes = new Dictionary<string, MainFrameSourceProjectTree>();
            string repositoryFolderName = new DirectoryInfo(rootDirectoryName).Name;

            foreach (var file in files)
            {
                var relativePath = GetRelativePath(file.FullPath, rootDirectoryName);
                var directoryPath = Path.GetDirectoryName(relativePath);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    var folderNode = AddToTree(directoryPath, rootNode, folderNodes);
                    var fileNode = CreateSourceProjectFileNode(file);

                    if (!folderNode.Children.Any(child => child.Name == fileNode.Name))
                    {
                        folderNode.Children.Add(fileNode);
                    }
                }
                else
                {
                    var fileNode = CreateSourceProjectFileNode(file);
                    if (!rootNode.Children.Any(child => child.Name == fileNode.Name))
                    {
                        rootNode.Children.Add(fileNode);
                    }
                }
            }

            SetAndConcatenateFolderIds(rootNode);
        }
        private void BuildDestinationProjectWorkspaceTree(MainFrameSourceProjectTree rootNode, List<MainFrameDestinationFiles> files, string rootDirectoryName, int projectId)
        {
            var folderNodes = new Dictionary<string, MainFrameSourceProjectTree>();
            string repositoryFolderName = new DirectoryInfo(rootDirectoryName).Name;

            foreach (var file in files)
            {
                var relativePath = GetRelativePath(file.FullPath, rootDirectoryName);
                var directoryPath = Path.GetDirectoryName(relativePath);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    var folderNode = AddToTree(directoryPath, rootNode, folderNodes);
                    var fileNode = CreateDestinationProjectFileNode(file);

                    if (!folderNode.Children.Any(child => child.Name == fileNode.Name))
                    {
                        folderNode.Children.Add(fileNode);
                    }
                }
                else
                {
                    var fileNode = CreateDestinationProjectFileNode(file);
                    if (!rootNode.Children.Any(child => child.Name == fileNode.Name))
                    {
                        rootNode.Children.Add(fileNode);
                    }
                }
            }

            SetAndConcatenateFolderIds(rootNode);
        }
        private void SetAndConcatenateFolderIds(MainFrameSourceProjectTree node, bool isRoot = true)
        {
            if (node.Children == null || node.Children.Count == 0)
            {
                return;
            }

            foreach (var child in node.Children)
            {
                SetAndConcatenateFolderIds(child, false);
            }

            if (!isRoot)
            {
                var nonEmptyChildIds = node.Children.Where(c => !string.IsNullOrEmpty(c.Id)).Select(c => c.Id).ToList();
                if (nonEmptyChildIds.Any())
                {
                    node.Id = string.Join(",", nonEmptyChildIds);
                }
            }
        }
        private MainFrameSourceProjectTree AddToTree(string directoryPath, MainFrameSourceProjectTree parent, Dictionary<string, MainFrameSourceProjectTree> folderNodes)
        {
            var pathParts = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var currentNode = parent;

            foreach (var part in pathParts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                var fullPath = Path.Combine(currentNode.Name, part);
                if (!folderNodes.TryGetValue(fullPath, out var folderNode))
                {
                    folderNode = new MainFrameSourceProjectTree
                    {
                        Id = "",
                        Name = part,
                        Type = "folder",
                        Children = new List<MainFrameSourceProjectTree>()
                    };
                    folderNodes[fullPath] = folderNode;
                    currentNode.Children.Add(folderNode);
                }
                currentNode = folderNode;

            }

            return currentNode;
        }
        //private string GetRelativePath(string fullPath, string rootDirectoryName)
        //{
        //    string repositoryFolderName = new DirectoryInfo(rootDirectoryName).Name;
        //    int index = fullPath.IndexOf(repositoryFolderName, StringComparison.OrdinalIgnoreCase);

        //    if (index != -1)
        //    {
        //        return fullPath.Substring(index + repositoryFolderName.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        //    }

        //    return fullPath;
        //}
        private string GetRelativePath(string fullPath, string rootDirectoryName)
        {
            // Define possible folder names
            string[] folderMarkers = { "/Files/", "/ZIP/", "/Library/" };

            foreach (var marker in folderMarkers)
            {
                int index = fullPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    // Extract everything after the marker
                    return fullPath.Substring(index + marker.Length);
                }
            }

            // If no marker found, return the full path as fallback
            return fullPath;
        }
        private MainFrameSourceProjectTree CreateSourceProjectFileNode(MainFrameSourceFiles file)
        {
            var destinationFiles = _dbContextGenAiPOC.MainFrameDestinationFiles.Where(x => x.MainFrameSourceFilesId.Equals(file.Id)).ToList();
            return new MainFrameSourceProjectTree
            {
                Id = file.Id.ToString(),
                Name = Path.GetFileName(file.Name),
                Type = "file",
                FileType = Path.GetExtension(file.Name),
                Content = file.FullPath != null ? _blobStorageService.GetBlobContentAsync(file.FullPath).Result : "",
                DestinationFiles = destinationFiles ?? null,
                Children = new List<MainFrameSourceProjectTree>()
            };
        }
        private MainFrameSourceProjectTree CreateDestinationProjectFileNode(MainFrameDestinationFiles file)
        {

            return new MainFrameSourceProjectTree
            {
                Id = file.Id.ToString(),
                Name = Path.GetFileName(file.ChunkFileName),
                Type = "file",
                IsSummarized = file.Status == SummarizeMainFrame.Summarized ? true : false,
                Summary = file.Summary,
                Content = file.FullPath != null ? _blobStorageService.GetBlobContentAsync(file.FullPath).Result : "",
                FileType = Path.GetExtension(file.ChunkFileName),
                Children = new List<MainFrameSourceProjectTree>()
            };
        }
        private async Task<string> GetMainFrameDestinationProjectFullPath(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            var parts = url.Split('/');

            int filesIndex = Array.IndexOf(parts, "Files");

            if (filesIndex == -1 || filesIndex + 1 >= parts.Length)
                return string.Empty;

            return string.Join("/", parts.Take(filesIndex + 2));
        }
        private string ConvertToApiUrl(string repoUrl)
        {
            if (repoUrl.Contains("github.com"))
            {
                // Remove `.git` if it exists at the end of the URL
                repoUrl = repoUrl.EndsWith(".git") ? repoUrl[..^4] : repoUrl;

                // Extract the owner and repo name
                Uri uri = new Uri(repoUrl);
                string[] segments = uri.AbsolutePath.Trim('/').Split('/');

                if (segments.Length < 2)
                    throw new ArgumentException("Invalid GitHub repository URL format.");

                return $"https://api.github.com/repos/{segments[0]}/{segments[1]}";
            }

            if (repoUrl.Contains("gitlab.com"))
                return repoUrl.Replace("gitlab.com", "gitlab.com/api/v4/projects/")
                              .Replace("https://", "");

            if (repoUrl.Contains("dev.azure.com"))
                return repoUrl + "?api-version=6.0";

            throw new NotSupportedException("Unsupported repository platform.");
        }

        private async Task InitializeBranchInRepo(ExportMainFrameProjectByURLServiceDTO dto, string repoPath)
        {
            try
            {
                Console.WriteLine($"Checking repository access: {dto.RepoURL}");

                // Initialize the repository directory
                Repository.Init(repoPath);
                using var repo = new Repository(repoPath);

                var remoteName = "origin";

                // Ensure the remote exists
                var remote = repo.Network.Remotes[remoteName] ?? repo.Network.Remotes.Add(remoteName, dto.RepoURL);

                // 🔹 Ensure at least one commit exists (Git requires an initial commit)
                if (!repo.Commits.Any())
                {
                    Console.WriteLine("Repository is empty. Creating initial commit...");

                    string readmePath = Path.Combine(repoPath, "README.md");
                    await File.WriteAllTextAsync(readmePath, $"# {dto.RepoBranch} branch initialized");

                    // Stage & commit
                    Commands.Stage(repo, readmePath);
                    var author = new Signature(dto.CreatedBy ?? "", $"{dto.CreatedBy}@users.noreply.github.com", DateTimeOffset.Now);
                    var commit = repo.Commit($"Initialize repository with {dto.RepoBranch} branch", author, author);

                    // Create new branch from the initial commit
                    var newBranch = repo.CreateBranch(dto.RepoBranch, commit);
                    if (newBranch == null)
                    {
                        Console.WriteLine($"Failed to create branch {dto.RepoBranch}");
                        return;
                    }

                    // Set upstream tracking
                    repo.Branches.Update(newBranch,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = $"refs/heads/{dto.RepoBranch}");

                    // Checkout the new branch
                    Commands.Checkout(repo, newBranch);

                    // Push the new branch
                    Console.WriteLine($"Pushing branch '{dto.RepoBranch}'...");
                    PushToRemote(repo, newBranch, dto);
                }
                else
                {
                    Console.WriteLine($"Repository already contains commits. Checking branch '{dto.RepoBranch}'...");

                    // Check if the branch already exists
                    var existingBranch = repo.Branches[dto.RepoBranch] ?? repo.CreateBranch(dto.RepoBranch, repo.Head.Tip);
                    if (existingBranch == null)
                    {
                        Console.WriteLine($"Failed to create or find branch {dto.RepoBranch}");
                        return;
                    }

                    repo.Branches.Update(existingBranch,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = $"refs/heads/{dto.RepoBranch}");

                    // Checkout the branch
                    Commands.Checkout(repo, existingBranch);

                    // Push the branch
                    Console.WriteLine($"Pushing branch '{dto.RepoBranch}'...");
                    PushToRemote(repo, existingBranch, dto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        private bool DoesBranchExist(ExportMainFrameProjectByURLServiceDTO dto)
        {
            try
            {
                using var repo = new Repository(Repository.Init(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()), false));

                var remote = repo.Network.Remotes["origin"] ?? repo.Network.Remotes.Add("origin", dto.RepoURL);

                var fetchOptions = new FetchOptions();

                if (dto.Platform == PlatformType.GitHub)
                {
                    fetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoPAT,
                        Password = ""
                    };
                }
                else if (dto.Platform == PlatformType.AzureDevops)
                {
                    fetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoUserName,
                        Password = dto.RepoPAT
                    };
                }
                else if (dto.Platform == PlatformType.GitLab)
                {
                    fetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoUserName,
                        Password = dto.RepoPAT
                    };
                }
                repo.Network.Fetch(remote.Name, new[] { "refs/heads/*:refs/remotes/origin/*" }, fetchOptions);
                return repo.Branches.Any(b => b.FriendlyName == $"origin/{dto.RepoBranch}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private void PushToRemote(Repository repo, Branch branch, ExportMainFrameProjectByURLServiceDTO dto)
        {
            try
            {
                var pushOptions = new PushOptions();

                if (dto.Platform == PlatformType.GitHub)
                {
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoPAT,
                        Password = ""
                    };
                }
                else if (dto.Platform == PlatformType.AzureDevops)
                {
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoUserName,
                        Password = dto.RepoPAT
                    };
                }
                else if (dto.Platform == PlatformType.GitLab)
                {
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = dto.RepoUserName,
                        Password = dto.RepoPAT
                    };
                }

                repo.Network.Push(branch, pushOptions);
            }
            catch (LibGit2SharpException ex)
            {
                Console.WriteLine($"Push failed: {ex.Message}");
                throw;
            }
        }
        public bool IsBranchEmpty(Repository repo, Branch branch)
        {

            var remoteBranch = repo.Branches[$"origin/{branch.FriendlyName}"];

            if (remoteBranch == null || remoteBranch.Tip == null)
                return true;

            var latestPushedCommit = remoteBranch.Tip;
            var treeEntries = latestPushedCommit.Tree;

            return treeEntries.All(entry => entry.Path.Equals("README.md", StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> StageAndCommitFiles(Repository repo, Dictionary<string, byte[]> files, string commitMessage, ExportMainFrameProjectByURLServiceDTO dto)
        {
            bool isSuccess = false;
            try
            {
                foreach (var file in files)
                {
                    string filePath = Path.Combine(repo.Info.WorkingDirectory, file.Key);

                    string? directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    await File.WriteAllBytesAsync(filePath, file.Value);

                    Commands.Stage(repo, filePath);
                }

                Signature author = new Signature(dto.CreatedBy ?? "", $"{dto.CreatedBy}@users.noreply.github.com", DateTimeOffset.Now);

                repo.Commit(commitMessage, author, author);

                return isSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StageAndCommitFiles: {ex.Message}");
                return isSuccess;
            }
        }

        public async Task<bool> PushMainFrameProjectChanges(Repository repo, Branch branch, ExportMainFrameProjectByURLServiceDTO dto)
        {
            bool isSuccess = false;
            try
            {
                var remote = repo.Network.Remotes["origin"];
                var credentials = new UsernamePasswordCredentials();

                if (dto.Platform == PlatformType.GitHub)
                {
                    credentials.Username = dto.RepoPAT;
                    credentials.Password = "";
                }
                else if (dto.Platform == PlatformType.AzureDevops)
                {
                    credentials.Username = dto.RepoUserName;
                    credentials.Password = dto.RepoPAT;
                }
                else if (dto.Platform == PlatformType.GitLab)
                {
                    credentials.Username = dto.RepoUserName;
                    credentials.Password = dto.RepoPAT;
                }
                var pushOptions = new PushOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => credentials
                };

                var fetchOptions = new FetchOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => credentials
                };
                if (branch.TrackedBranch == null)
                {
                    Console.WriteLine($"Setting upstream tracking for {branch.FriendlyName}");
                    repo.Branches.Update(branch,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = $"refs/heads/{branch.FriendlyName}");
                }

                Commands.Fetch(repo, remote.Name, new string[] { branch.UpstreamBranchCanonicalName }, fetchOptions, null);

                var remoteBranch = repo.Branches[$"origin/{branch.FriendlyName}"];

                if (remoteBranch != null)
                {
                    Console.WriteLine("Merging remote changes into local branch...");
                    var mergeResult = repo.Merge(remoteBranch, new Signature("Your Name", "your-email@example.com", DateTimeOffset.Now));

                    if (mergeResult.Status == MergeStatus.Conflicts)
                    {
                        Console.WriteLine("Merge conflicts detected! Resolve conflicts manually before pushing.");
                        return isSuccess;
                    }
                }

                Console.WriteLine($"Pushing {branch.FriendlyName} to {remote.Url}");
                repo.Network.Push(branch, pushOptions);
                Console.WriteLine("Push successful!");

                return isSuccess = true;
            }
            catch (LibGit2Sharp.NonFastForwardException)
            {
                Console.WriteLine("Push failed: Non-fast-forward update. Perform a pull first.");
            }
            catch (LibGit2Sharp.LibGit2SharpException ex)
            {
                Console.WriteLine($"Push failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return isSuccess;
        }


        #endregion
    }
}