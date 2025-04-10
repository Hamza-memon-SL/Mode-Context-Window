using AutoMapper;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using GenAiPoc.Infrastructure.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Application.Services
{

    public class MainFrameService : IMainFrameService
    {
        private readonly ILogger<MainFrameService> _logger;
        private readonly IMainFrameRepository _mainFrameRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;

        public MainFrameService(ILogger<MainFrameService> logger, IMainFrameRepository _mainFrameRepository, IBlobStorageService blobStorageService, IMapper mapper)
        {
            _logger = logger;
            this._mainFrameRepository = _mainFrameRepository;
            this._blobStorageService = blobStorageService;
            _mapper = mapper;

        }

        public async Task<ImportResponse> ImportMainFrameProjectByURLService(ImportMainFrameProjectDTO request)
        {

            try
            {
                ImportResponse response;
                var project = await _mainFrameRepository.GetMainFrameSourceProjectByRepoUrlOrByFile(request, null);
                if (project == null)
                {
                    var localPath = await _mainFrameRepository.GetTempMainFrameSourceFolderPath();
                    (bool isSuccess, bool isPrivate, bool isBranchExist, string msg) cloneSuccess = (false, request.isPrivate, false, "");
                    //if (request.isPrivate == false)
                    cloneSuccess = await _mainFrameRepository.ClonePublicMainFrameSourceRepository(request, localPath);
                    //else if (request.isPrivate == true)
                    //    cloneSuccess = await _codeBuddyRepository.ClonePrivateRepository(request, localPath);
                    if (cloneSuccess.isSuccess == true)
                    {
                        response = await _mainFrameRepository.ProcessMainFrameSourceProjectByURLAsync(request, localPath);

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

        public async Task<ImportResponse> ImportMainFrameProjectByZipOrFileService(ImportMainFrameProjectDTO request, List<IFormFile>? files)
        {

            try
            {
                ImportResponse response;

                var project = await _mainFrameRepository.GetMainFrameSourceProjectByRepoUrlOrByFile(request, files);
                if (project == null)
                {
                    var zipOrFilesExist = _mainFrameRepository.HasValidZipOrFiles(files);
                    if (zipOrFilesExist)
                    {
                        response = await _mainFrameRepository.ProcessMainFrameSourceProjectByZipOrFileAsync(request, files);

                        if (response.Status == 0)
                        {
                            return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, response.Message);
                        }
                        else
                        {
                            return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, response.Message);
                        }
                    }
                    else
                    {
                        return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ZipOrInvalidFile);
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
        public async Task<GetMainFrameProjectDetailsByAuthTokenResponse> GetMainFrameProjectDetailsByAuthTokenService(string authToken)
        {

            try
            {
                var response = await _mainFrameRepository.GetMainFrameProjectDetailsByAuthTokenAsync(authToken);
                if (response != null)
                {
                    return new GetMainFrameProjectDetailsByAuthTokenResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetMainFrameProjectDetailsByAuthTokenResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetMainFrameProjectDetailsByAuthTokenResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }

        }
        public async Task<EditMainFrameSourceProjectResponse> EditMainFrameSourceProjectService(EditMainFrameSourceProjectDTO request)
        {
            try
            {
                var response = await _mainFrameRepository.EditMainFrameSourceProjectAsync(request);
                if (response)
                {
                    return new EditMainFrameSourceProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new EditMainFrameSourceProjectResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new EditMainFrameSourceProjectResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<DeleteWorkspaceResponse> DeleteMainFrameSourceProjectService(int projectId)
        {
            try
            {
                var response = await _mainFrameRepository.DeleteMainFrameSourceProjectServiceAsync(projectId);
                if (response)
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.DeleteFailed);
                }
            }
            catch (Exception ex)
            {
                return new DeleteWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }


        public async Task<GetMainFrameProjectDetailsResponse> GetMainFrameProjectDetailsService(int itemId, string itemType, string projectType)
        {
            try
            {
                var response = await _mainFrameRepository.GetMainFrameProjectDetailsAsync(itemId, itemType, projectType);
                if (response != null)
                {
                    return new GetMainFrameProjectDetailsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetMainFrameProjectDetailsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetMainFrameProjectDetailsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetMainFrameSourceProjectResponse> GetMainFrameSourceProjectService(int projectId)
        {

            try
            {
                var response = await _mainFrameRepository.GetMainFrameSourceProjectAsync(projectId);
                if (response.Count > 0)
                {
                    return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }

        }
        public async Task<GetMainFrameSourceProjectResponse> GetMainFrameDestinationProjectService(int projectId)
        {

            try
            {
                var response = await _mainFrameRepository.GetMainFrameDestinationProjectAsync(projectId);
                if (response.Count > 0)
                {
                    return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, response);
                }
                else
                {
                    return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, response);
                }
            }
            catch (Exception ex)
            {
                return new GetMainFrameSourceProjectResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }

        }
        public async Task<ImportResponse> CreateMainFrameChunkFileService(CreateMainFrameChunkFileDTO request)
        {
            try
            {
                ImportResponse response;
                var mainFrameDestinationProject = await _mainFrameRepository.GetMainFrameDestinationProjectByIdAsync(request);
                if (mainFrameDestinationProject != null)
                {
                    var mainFrameDestinationFile = await _mainFrameRepository.GetMainFrameDestionationProjectAllFileDetails(request, mainFrameDestinationProject);
                    if (mainFrameDestinationFile != null)
                    {
                        var uploadMainFrameDestinationFileToBlob = await _blobStorageService.UploadMainFrameDestinationProjectByFileToBlob(mainFrameDestinationFile, request);
                        if (uploadMainFrameDestinationFileToBlob != null)
                        {
                            bool filesSaved = await _mainFrameRepository.SaveMainFrameDestinationFileDetailsToDatabase(mainFrameDestinationFile);
                            if (filesSaved && string.IsNullOrEmpty(mainFrameDestinationProject.FullPath))
                            {
                                var updateDestinationProject = await _mainFrameRepository.UpdateMainFrameDestinationProjectAsync(mainFrameDestinationFile);
                                return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);

                            }
                            if (filesSaved)
                            {
                                return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.FileSavedSuccessfully);
                            }
                        }
                        else
                        {
                            return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.FileSavedFailedBlob);
                        }
                    }
                }
                else
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoDataFound);
                }
                return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoDataFound);
            }
            catch (Exception ex)
            {
                return new ImportResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }


        }
        public async Task<ImportResponse> MoveMainFrameChunkFileService(int destinationFileId, string destinationFilePath)
        {
            try
            {
                ImportResponse response;
                var mainFrameDestinationProjectFile = await _mainFrameRepository.GetMainFrameDestinationProjectFileByIdAsync(destinationFileId);
                if (mainFrameDestinationProjectFile != null)
                {
                    var moveMainFrameDestinationProjectBlobFile = await _blobStorageService.MoveMainFrameDestinationProjectBlobFileAsync(mainFrameDestinationProjectFile.FullPath, destinationFilePath);
                    if (moveMainFrameDestinationProjectBlobFile != null)
                    {
                        var updateDestinationProject = await _mainFrameRepository.UpdateMainFrameDestinationProjectFileAsync(mainFrameDestinationProjectFile, destinationFilePath);
                        if (updateDestinationProject)
                        {
                            return new ImportResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                        }
                        else
                        {
                            return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                        }
                    }
                    else
                    {
                        return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                    }
                }
                else
                {
                    return new ImportResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoDataFound);
                }
            }
            catch (Exception ex)
            {
                return new ImportResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
        public async Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByZipService(int destinationProjectId)
        {
            try
            {

                var mainFrameDestinationProject = await _mainFrameRepository.GetMainFrameDestinationProjectByIdAsync(destinationProjectId);
                if (mainFrameDestinationProject != null)
                {
                    var downloadMainFrameDestinationProjectByZip = await _blobStorageService.ExportMainFrameProjectByZipBlobAsync(mainFrameDestinationProject.FullPath);
                    if (downloadMainFrameDestinationProjectByZip != null)
                    {
                        return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, downloadMainFrameDestinationProjectByZip, mainFrameDestinationProject?.Name, "application/zip");
                    }
                    else
                    {
                        return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoDataFound, null, null, null);
                    }
                }
                else
                {
                    return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage, null, null, null);
                }
            }
            catch (Exception ex)
            {
                return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null, null, null);
            }
        }
        public async Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByURLService(ExportMainFrameProjectByURLServiceDTO exportMainFrameProjectByURLServiceDTO)
        {
            try
            {

                var mainFrameDestinationProject = await _mainFrameRepository.GetMainFrameDestinationProjectByIdAsync(exportMainFrameProjectByURLServiceDTO.DestinationProjectId);
                if (mainFrameDestinationProject != null)
                {

                    var localPath = await _mainFrameRepository.GetTempMainFrameDestinationFolderPath();
                    using var repo = await _mainFrameRepository.ExportCloneMainFrameRepositoryToLocalDirectory(exportMainFrameProjectByURLServiceDTO, localPath);
                    Branch branch = repo.Branches[exportMainFrameProjectByURLServiceDTO.RepoBranch];
                    if (!_mainFrameRepository.IsBranchEmpty(repo, branch))
                    {
                        return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProvideEmptyBranch, null, null, null);
                    }
                    else
                    {
                        Dictionary<string, byte[]> blobFiles = await _blobStorageService.ExportMainFrameProjectByURLBlobAsync(mainFrameDestinationProject.FullPath);
                        if (blobFiles.Count == 0)
                        {
                            return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoFilesFound, null, null, null);
                        }
                        else
                        {
                            var stageCheck = await _mainFrameRepository.StageAndCommitFiles(repo, blobFiles, $"Initial commit for branch {exportMainFrameProjectByURLServiceDTO.RepoBranch}, from Blob Storage at {DateTime.UtcNow.ToString()}", exportMainFrameProjectByURLServiceDTO);
                            var pushComplete = await _mainFrameRepository.PushMainFrameProjectChanges(repo, branch, exportMainFrameProjectByURLServiceDTO);
                            if (pushComplete)
                            {
                                await _mainFrameRepository.DeleteDirectory(localPath);
                                return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.ProjectEmported, null, null, null);
                            }
                            else
                            {
                                return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ProjectEmportedFailed, null, null, null);
                            }
                        }
                    }
                }
                else
                {
                    return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.NoDataFound, null, null, null);
                }
            }
            catch (Exception ex)
            {
                return new ExportMainFrameProjectByZipResponse(StatusAndMessagesKeys.InternalServerErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null, null, null);
            }
        }
        public async Task<DeleteWorkspaceResponse> DeleteMainFrameDestinationFiles(List<int> filesId)
        {
            try
            {
                var response = await _mainFrameRepository.DeleteMainFrameDestinationFilesServiceAsync(filesId);
                if (response)
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new DeleteWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.DeleteFailed);
                }
            }
            catch (Exception ex)
            {
                return new DeleteWorkspaceResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }
    }
}