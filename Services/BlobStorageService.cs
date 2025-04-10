using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using LibGit2Sharp;
using GenAiPoc.Api.Configurations;
using GenAiPoc.Contracts.Models;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.Concurrent;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph.Models.Security;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Response;
using System.IO.Compression;
using static Microsoft.Graph.Constants;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.Partners.Billing;
using System.Web.Http;

namespace GenAiPoc.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {

        private readonly string _blobContainerNameForCodeBuddyProject;
        private readonly string _blobContainerNameForMainFrameSourceProject;
        private readonly string _blobContainerNameForMainFrameDestinationProject;
        private readonly string _blobContainerNameForBuild;
        private readonly string _templateImage;
        private readonly string _blobUrl;
        private readonly string _interviewResumes;
        private readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;

        public BlobStorageService(BlobServiceClient blobServiceClient, string blobContainerNameForCodeBuddyProject, string blobContainerNameForMainFrameSourceProject, string blobContainerNameForMainFrameDestinationProject, string blobContainerNameForBuild, string templateImage, string interviewResumes, string blobUrl)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerNameForCodeBuddyProject = blobContainerNameForCodeBuddyProject;
            _blobContainerNameForMainFrameSourceProject = blobContainerNameForMainFrameSourceProject;
            _blobContainerNameForMainFrameDestinationProject = blobContainerNameForMainFrameDestinationProject;
            _blobContainerNameForBuild = blobContainerNameForBuild;
            _templateImage = templateImage;
            _interviewResumes = interviewResumes;
            _blobUrl = blobUrl;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForCodeBuddyProject);

        }

        public async Task<List<CodeBuddyFileDetails>> UploadRepositoryToBlob(List<CodeBuddyFileDetails> fileDetailsList, CodeBuddyProjects projects, string localPath)
        {
            try
            {
                //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
                await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                foreach (var fileDetail in fileDetailsList)
                {

                    if (File.Exists(fileDetail.FullPath))
                    {

                        string relativePath = Path.GetRelativePath(localPath, fileDetail.FullPath).Replace("\\", "/");
                        string completePath = $"{projects.AuthToken}/{relativePath}";
                        var url = $"{_blobUrl}/{_blobContainerNameForCodeBuddyProject}/{completePath}";
                        var blobClient = _blobContainerClient.GetBlobClient(completePath);
                        using (var fileStream = File.OpenRead(fileDetail.FullPath))
                        {
                            try
                            {
                                await blobClient.UploadAsync(fileStream, overwrite: true);
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }

                        }
                        fileDetail.FullPath = url;
                        Console.WriteLine($"Uploaded: {fileDetail.FullPath} to Blob Storage.");
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fileDetail.FullPath}. Skipping upload.");
                    }
                }

                return fileDetailsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading files to blob storage: " + ex.Message);
                throw;
            }
        }
        public async Task<string> GetBlobContentAsync(string blobUri)
        {

            BlobClient blobClient = new BlobClient(new Uri(blobUri));

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadContentAsync();
                return response.Value.Content.ToString();
            }

            return string.Empty;
        }
        public async Task<bool> DeleteFolderAsync(string authToken, string folderName)
        {
            bool isDeleted = false;
            try
            {
                //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
                string fullPath = $"{authToken}/{folderName}".TrimEnd('/') + "/";
                if (!fullPath.EndsWith("/"))
                {
                    fullPath += "/";
                }
                List<Task> deleteTasks = new List<Task>();
                await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync(prefix: fullPath))
                {
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                    deleteTasks.Add(blobClient.DeleteIfExistsAsync());
                }
                await Task.WhenAll(deleteTasks);
                return isDeleted = true;
            }
            catch (Exception ex)
            {
                return isDeleted;
            }

        }
        public async Task<List<DetailedBlobItem>> ListBlobAsync(string folderName)
        {
            ConcurrentBag<DetailedBlobItem> blobFiles = new ConcurrentBag<DetailedBlobItem>(); // Thread-safe collection
            try
            {
                //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
                if (!folderName.EndsWith("/"))
                {
                    folderName += "/";
                }
                List<Task> tasks = new List<Task>();
                await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync(prefix: folderName))
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);

                    var detailedBlobItem = new DetailedBlobItem
                    {
                        BlobItem = blobItem,
                        BlobClient = blobClient
                    };

                    tasks.Add(Task.Run(() => blobFiles.Add(detailedBlobItem)));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw;
            }
            return blobFiles.ToList();
        }
        public bool IsFileModified(string blobFilePath, string localFilePath)
        {
            //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobFilePath);
                var properties = blobClient.GetProperties();
                FileInfo localFileInfo = new FileInfo(localFilePath);

                string blobFileName = Path.GetFileName(blobFilePath);
                string localFileName = localFileInfo.Name;

                bool isNameModified = localFileName != blobFileName;
                bool isSizeModified = localFileInfo.Length != properties.Value.ContentLength;

                return isNameModified || isSizeModified;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<string> UploadFileToBlob(string blobPath, string localFilePath)
        {
            //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);

            try
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobPath);
                using FileStream uploadFileStream = File.OpenRead(localFilePath);
                await blobClient.UploadAsync(uploadFileStream, true);
                string blobUrl = blobClient.Uri.ToString();

                uploadFileStream.Close();
                return blobUrl;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<bool> DeleteBlobFiles(List<CodeBuddyFileDetails> fileDetails)
        {
            bool isDeleted = false;
            try
            {
                foreach (var fileToDelete in fileDetails)
                {
                    string file = new Uri(fileToDelete.FullPath).AbsolutePath.Replace(_blobContainerNameForCodeBuddyProject, string.Empty).TrimStart('/');
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(file);
                    await blobClient.DeleteIfExistsAsync();
                }
                return isDeleted = true;
            }
            catch (Exception ex)
            {
                return isDeleted;
            }
        }
        public async Task<bool> DeleteBlobFile(string filePath)
        {
            bool isDeleted = false;
            try
            {
                string file = new Uri(filePath).AbsolutePath.Replace(_blobContainerNameForCodeBuddyProject, string.Empty).TrimStart('/');
                BlobClient blobClient = _blobContainerClient.GetBlobClient(file);
                await blobClient.DeleteIfExistsAsync();
                return isDeleted = true;
            }
            catch (Exception ex)
            {
                return isDeleted;
            }
        }
        public async Task<bool> DeleteMainFrameDestinationBlobFile(string filePath)
        {
            bool isDeleted = false;
            try
            {
                string decodedFilePath = Uri.UnescapeDataString(new Uri(filePath).AbsolutePath);
                string file = decodedFilePath.Replace(_blobContainerNameForMainFrameDestinationProject, string.Empty).TrimStart('/');
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameDestinationProject);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(file);
                await blobClient.DeleteIfExistsAsync();
                return isDeleted = true;
            }
            catch (Exception ex)
            {
                return isDeleted;
            }
        }
        public async Task<string> UploadFileToBlobAsync(IFormFile file, string subFilePath)
        {
            try
            {
                // Normalize the file path for Blob storage
                var blobPath = subFilePath.Replace("\\", "/");

                // Get the BlobContainerClient for the specified container
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForBuild);
                var blobClient = _blobContainerClient.GetBlobClient(blobPath);
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);
                string blobUrl = blobClient.Uri.ToString();

                // Return the URL of the uploaded file
                return blobUrl;
            }
            catch (Exception ex)
            {
                // Log the error (optional) or rethrow if needed
                Console.WriteLine($"Error uploading file to blob storage: {ex.Message}");
                throw;
            }
        }
        public async Task<string> UploadTemplateImageToBlobAsync(IFormFile file, string subFilePath)
        {
            try
            {
                // Normalize the file path for Blob storage
                var blobPath = subFilePath.Replace("\\", "/");

                // Get the BlobContainerClient for the specified container
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_templateImage);
                var blobClient = _blobContainerClient.GetBlobClient(blobPath);
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);
                string blobUrl = blobClient.Uri.ToString();

                // Return the URL of the uploaded file
                return blobUrl;
            }
            catch (Exception ex)
            {
                // Log the error (optional) or rethrow if needed
                Console.WriteLine($"Error uploading file to blob storage: {ex.Message}");
                throw;
            }
        }
        public async Task<List<string>> UploadInterviewResumesToBlobAsync(List<IFormFile> files)
        {
            List<string> uploadedUrls = new List<string>();

            try
            {

                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_interviewResumes);

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                        //var uniqueFileName = file.FileName;
                        var blobPath = $"{uniqueFileName}";

                        var blobClient = _blobContainerClient.GetBlobClient(blobPath);
                        using var stream = file.OpenReadStream();
                        await blobClient.UploadAsync(stream, overwrite: true);

                        uploadedUrls.Add(blobClient.Uri.ToString());
                    }
                }

                return uploadedUrls;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading files to blob storage: {ex.Message}");
                throw;
            }
        }
        public async Task<List<MainFrameSourceFiles>> UploadMainFrameSourceProjectByURLToBlob(List<MainFrameSourceFiles> fileDetailsList, MainFrameSourceProject projects, string localPath)
        {
            try
            {
                //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameSourceProject);
                await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                foreach (var fileDetail in fileDetailsList)
                {

                    if (File.Exists(fileDetail.FullPath))
                    {

                        string relativePath = Path.GetRelativePath(localPath, fileDetail.FullPath).Replace("\\", "/");
                        string completePath = $"{projects.CreatedBy}/{"Library"}/{relativePath}";
                        var url = $"{_blobUrl}/{_blobContainerNameForMainFrameSourceProject}/{completePath}";
                        var blobClient = _blobContainerClient.GetBlobClient(completePath);
                        using (var fileStream = File.OpenRead(fileDetail.FullPath))
                        {
                            try
                            {
                                await blobClient.UploadAsync(fileStream, overwrite: true);
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }

                        }
                        fileDetail.FullPath = url;
                        Console.WriteLine($"Uploaded: {fileDetail.FullPath} to Blob Storage.");
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fileDetail.FullPath}. Skipping upload.");
                    }
                }

                return fileDetailsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading files to blob storage: " + ex.Message);
                throw;
            }
        }
        public async Task<MainFrameDestinationFiles> UploadMainFrameDestinationProjectByFileToBlob(MainFrameDestinationFiles mainFrameDestinationFiles, CreateMainFrameChunkFileDTO request)
        {
            try
            {
                //var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameDestinationProject);
                await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                string relativePath = $"{mainFrameDestinationFiles.AuthToken}/Files{mainFrameDestinationFiles.FullPath}";
                string completePath = $"{_blobUrl}/{_blobContainerNameForMainFrameDestinationProject}/{relativePath}";

                var blobClient = _blobContainerClient.GetBlobClient(relativePath);

                using (var stream = request.MainFrameDestinationFile.OpenReadStream())
                {
                    try
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error uploading file: {ex.Message}");
                        throw;
                    }
                }

                mainFrameDestinationFiles.FullPath = completePath;
                Console.WriteLine($"Uploaded: {mainFrameDestinationFiles.FullPath} to Blob Storage.");

                return mainFrameDestinationFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file to blob storage: " + ex.Message);
                throw;
            }
        }
        public async Task<List<MainFrameSourceFiles>> UploadMainFrameSourceProjectZipOrFileToBlob(List<IFormFile> files, MainFrameSourceProject project)
        {
            try
            {
                var uploadedFiles = new List<MainFrameSourceFiles>();

                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameSourceProject);
                await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var zipFile = files.FirstOrDefault(f => Path.GetExtension(f.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase));

                if (zipFile != null)
                {
                    string zipFolderName = Path.GetFileNameWithoutExtension(zipFile.FileName);
                    using (var zipStream = zipFile.OpenReadStream())
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        if (!archive.Entries.Any())
                        {
                            Console.WriteLine("ZIP file is empty.");
                            return uploadedFiles;
                        }

                        foreach (var entry in archive.Entries)
                        {
                            if (!string.IsNullOrWhiteSpace(entry.Name))
                            {
                                using (var entryStream = entry.Open())
                                using (var memoryStream = new MemoryStream())
                                {
                                    await entryStream.CopyToAsync(memoryStream);
                                    memoryStream.Position = 0;

                                    string relativePath = entry.FullName.Replace("\\", "/");
                                    string blobPath = $"{project.AuthToken}/{"ZIP"}/{zipFolderName}/{relativePath}";

                                    var blobClient = _blobContainerClient.GetBlobClient(blobPath);
                                    await blobClient.UploadAsync(memoryStream, overwrite: true);

                                    string fileUrl = $"{_blobUrl}/{_blobContainerNameForMainFrameSourceProject}/{blobPath}";
                                    var fileInfo = new FileInfo(entry.Name);
                                    using (var lineCountStream = new MemoryStream(memoryStream.ToArray()))
                                    {
                                        uploadedFiles.Add(new MainFrameSourceFiles
                                        {
                                            Name = fileInfo.Name,
                                            FullPath = fileUrl,
                                            Extension = fileInfo.Extension,
                                            Size = FormatFileSize(entry.Length),
                                            CreatedDate = DateTime.UtcNow,
                                            ModifiedDate = DateTime.UtcNow,
                                            MainFrameSourceProjectId = project.Id,
                                            CreatedBy = project?.CreatedBy,
                                            AuthToken = project?.AuthToken,
                                            LineCount = GetLineCountFromStream(lineCountStream),
                                            IsActive = true,
                                        });
                                    }

                                    Console.WriteLine($"Uploaded: {fileUrl}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    string projectFolder = $"{project.AuthToken}/{"Files"}/{project.Name}";
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            string blobPath = $"{projectFolder}/{file.FileName}";
                            var blobClient = _blobContainerClient.GetBlobClient(blobPath);

                            using (var memoryStream = new MemoryStream())
                            {
                                await file.OpenReadStream().CopyToAsync(memoryStream);
                                memoryStream.Position = 0;
                                await blobClient.UploadAsync(memoryStream, overwrite: true);

                                string fileUrl = $"{_blobUrl}/{_blobContainerNameForMainFrameSourceProject}/{blobPath}";
                                var fileInfo = new FileInfo(file.FileName);

                                using (var lineCountStream = new MemoryStream(memoryStream.ToArray()))
                                {
                                    uploadedFiles.Add(new MainFrameSourceFiles
                                    {
                                        Name = fileInfo.Name,
                                        FullPath = fileUrl,
                                        Extension = fileInfo.Extension,
                                        Size = FormatFileSize(file.Length),
                                        CreatedDate = DateTime.UtcNow,
                                        ModifiedDate = DateTime.UtcNow,
                                        MainFrameSourceProjectId = project.Id,
                                        CreatedBy = project?.CreatedBy,
                                        AuthToken = project?.AuthToken,
                                        LineCount = GetLineCountFromStream(lineCountStream),
                                        IsActive = true,
                                    });
                                }

                                Console.WriteLine($"Uploaded: {fileUrl}");
                            }
                        }
                    }
                }
                return uploadedFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading files to blob storage: " + ex.Message);
                throw;
            }
        }
        public async Task<string> MoveMainFrameDestinationProjectBlobFileAsync(string oldFilePath, string newFilePath)
        {
            try
            {
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameDestinationProject);

                oldFilePath = await ExtractBlobFilePath(oldFilePath);
                newFilePath = await ExtractBlobFilePath(newFilePath);

                var oldFilePathBlobClient = _blobContainerClient.GetBlobClient(oldFilePath);
                var newFilePathBlobClient = _blobContainerClient.GetBlobClient(newFilePath);

                if (await oldFilePathBlobClient.ExistsAsync())
                {

                    await newFilePathBlobClient.StartCopyFromUriAsync(oldFilePathBlobClient.Uri);
                    await oldFilePathBlobClient.DeleteIfExistsAsync();

                    string newFullUri = newFilePathBlobClient.Uri.ToString();

                    return newFullUri;
                }
                else
                {
                    Console.WriteLine($"Source file does not exist: {oldFilePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving file: {ex.Message}");
                return null;
            }
        }
        public async Task<byte[]> ExportMainFrameProjectByZipBlobAsync(string projectPath)
        {

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameDestinationProject);
                projectPath = await ExtractBlobFilePath(projectPath);

                var newProjectPath = _blobContainerClient.GetBlobClient(projectPath);

                var blobs = containerClient.GetBlobsAsync(prefix: newProjectPath.Name);
                var memoryStream = new MemoryStream();

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    await foreach (var blobItem in blobs)
                    {
                        var blobClient = containerClient.GetBlobClient(blobItem.Name);

                        var relativePath = blobItem.Name.Substring(projectPath.LastIndexOf('/'));
                        var entry = archive.CreateEntry(relativePath, CompressionLevel.Fastest);


                        using (var entryStream = entry.Open())
                        using (var blobStream = await blobClient.OpenReadAsync())
                        {
                            await blobStream.CopyToAsync(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;
                byte[] zipBytes = memoryStream.ToArray();
                return zipBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating zip file: {ex.Message}");
                return null;

            }
        }

        public async Task<Dictionary<string, byte[]>> ExportMainFrameProjectByURLBlobAsync(string projectPath)
        {
            projectPath = await ExtractBlobFilePath(projectPath);

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerNameForMainFrameDestinationProject);
                Dictionary<string, byte[]> fileContents = new Dictionary<string, byte[]>();

                projectPath = projectPath.Trim().TrimStart('/').TrimEnd('/');

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: projectPath))
                {
                    string blobName = blobItem.Name;

                    string relativePath = blobName.Substring(projectPath.Length).TrimStart('/');
                    if (string.IsNullOrWhiteSpace(relativePath)) continue;

                    BlobClient blob = containerClient.GetBlobClient(blobName);

                    using MemoryStream stream = new MemoryStream();
                    await blob.DownloadToAsync(stream);
                    fileContents[relativePath] = stream.ToArray();
                }

                return fileContents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in ExportMainFrameProjectByURLBlobAsync: {ex.Message}");
                throw;
            }
        }

        private static string FormatFileSize(long bytes)
        {
            return $"{bytes} bytes";
        }
        private int GetLineCountFromStream(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return 0;

            int lineCount = 0;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream); // Copy stream to memory
                memoryStream.Position = 0; // Reset before reading

                using (var reader = new StreamReader(memoryStream))
                {
                    while (reader.ReadLine() != null)
                    {
                        lineCount++;
                    }
                }
            }

            return lineCount;
        }
        public async Task<string> ExtractBlobFilePath(string blobUrl)
        {
            //string containerPrefix = "https://appcodeanalyzerstorage.blob.core.windows.net/mainframedestinationprojects/";
            string containerPrefix = $"{_blobUrl}/{_blobContainerNameForMainFrameDestinationProject}/";
            if (blobUrl.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return "/" + blobUrl.Substring(containerPrefix.Length);
            }

            return  blobUrl; 
        }

    }
}


