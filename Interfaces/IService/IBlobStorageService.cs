using Azure.Storage.Blobs.Models;
using GenAiPoc.Contracts.Models;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IBlobStorageService
    {

        Task<List<CodeBuddyFileDetails>> UploadRepositoryToBlob(List<CodeBuddyFileDetails> fileDetailsList, CodeBuddyProjects projects, string localPath);
        Task<string> GetBlobContentAsync(string blobUri);
        Task<bool> DeleteFolderAsync(string authToken, string folderName);
        Task<List<DetailedBlobItem>> ListBlobAsync(string folderName);
        bool IsFileModified(string blobFilePath, string localFilePath);
        Task<string> UploadFileToBlob(string blobPath, string localFilePath);
        Task<bool> DeleteBlobFiles(List<CodeBuddyFileDetails> fileDetails);
        Task<bool> DeleteBlobFile(string filePath);
        Task<string> UploadFileToBlobAsync(IFormFile file, string subFilePath);
        Task<string> UploadTemplateImageToBlobAsync(IFormFile file, string subFilePath);
        Task<List<string>> UploadInterviewResumesToBlobAsync(List<IFormFile> files);
        Task<List<MainFrameSourceFiles>> UploadMainFrameSourceProjectByURLToBlob(List<MainFrameSourceFiles> fileDetailsList, MainFrameSourceProject projects, string localPath);
        Task<List<MainFrameSourceFiles>> UploadMainFrameSourceProjectZipOrFileToBlob(List<IFormFile> files, MainFrameSourceProject project);
        Task<MainFrameDestinationFiles> UploadMainFrameDestinationProjectByFileToBlob(MainFrameDestinationFiles mainFrameDestinationFiles, CreateMainFrameChunkFileDTO request);
        Task<string> MoveMainFrameDestinationProjectBlobFileAsync(string oldFilePath, string newFilePath);
        Task<byte[]> ExportMainFrameProjectByZipBlobAsync(string projectPath);
        Task<Dictionary<string, byte[]>> ExportMainFrameProjectByURLBlobAsync(string projectPath);
        Task<bool> DeleteMainFrameDestinationBlobFile(string filePath);
    }
}


