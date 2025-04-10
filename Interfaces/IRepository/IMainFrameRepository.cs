using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Response;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface IMainFrameRepository
    {
        Task<MainFrameSourceProject?> GetMainFrameSourceProjectByRepoUrlOrByFile(ImportMainFrameProjectDTO model, List<IFormFile> files);
        Task<string> GetTempMainFrameSourceFolderPath();
        Task<(bool isSuccess, bool isPrivate, bool isBranchExist, string message)> ClonePublicMainFrameSourceRepository(ImportMainFrameProjectDTO dto, string localPath);
        Task<ImportResponse> ProcessMainFrameSourceProjectByURLAsync(ImportMainFrameProjectDTO dto, string localPath);
        Task<ImportResponse> ProcessMainFrameSourceProjectByZipOrFileAsync(ImportMainFrameProjectDTO dto, List<IFormFile> files);
        bool HasValidZipOrFiles(List<IFormFile> files);
        Task<List<MainFrameSourceProject>> GetMainFrameProjectDetailsByAuthTokenAsync(string authToken);
        Task<bool> EditMainFrameSourceProjectAsync(EditMainFrameSourceProjectDTO request);
        Task<bool> DeleteMainFrameSourceProjectServiceAsync(int projectId);
        Task<List<MainFrameSourceProjectTree>> GetMainFrameSourceProjectAsync(int projectId);
        Task<List<MainFrameSourceProjectTree>> GetMainFrameDestinationProjectAsync(int projectId);
        Task<GetMainFrameProjectDetails> GetMainFrameProjectDetailsAsync(int itemId, string itemType, string projectType);
        Task<MainFrameDestinationProject?> GetMainFrameDestinationProjectByIdAsync(CreateMainFrameChunkFileDTO request);
        Task<MainFrameDestinationFiles> GetMainFrameDestionationProjectAllFileDetails(CreateMainFrameChunkFileDTO request, MainFrameDestinationProject mainFrameDestinationProject);
        Task<bool> SaveMainFrameDestinationFileDetailsToDatabase(MainFrameDestinationFiles file);
        Task<bool> UpdateMainFrameDestinationProjectAsync(MainFrameDestinationFiles mainFrameDestinationFiles);
        Task<MainFrameDestinationFiles?> GetMainFrameDestinationProjectFileByIdAsync(int destinationFileId);
        Task<bool> UpdateMainFrameDestinationProjectFileAsync(MainFrameDestinationFiles mainFrameDestinationFile, string destinationFilePath);
        Task<MainFrameDestinationProject?> GetMainFrameDestinationProjectByIdAsync(int destinationProjectId);
        Task<bool> IsExportRepositoryPublic(string repoUrl);
        Task<Repository> ExportCloneMainFrameRepositoryToLocalDirectory(ExportMainFrameProjectByURLServiceDTO dto, string localPath);
        Task<Branch> GetOrCreateBranch(Repository repo, string branchName);
        bool IsBranchEmpty(Repository repo,Branch branch);
        Task<string> GetTempMainFrameDestinationFolderPath();
        Task<bool> PushMainFrameProjectChanges(Repository repo, Branch branch, ExportMainFrameProjectByURLServiceDTO dto);
        Task<bool> StageAndCommitFiles(Repository repo, Dictionary<string, byte[]> files, string commitMessage, ExportMainFrameProjectByURLServiceDTO exportMainFrameProjectByURLServiceDTO);
        Task DeleteDirectory(string targetDir);
        Task<bool> DeleteMainFrameDestinationFilesServiceAsync(List<int> filesId);
    }
}
