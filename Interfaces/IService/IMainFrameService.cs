using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IMainFrameService
    {
        Task<ImportResponse> ImportMainFrameProjectByURLService(ImportMainFrameProjectDTO request);
        Task<ImportResponse> ImportMainFrameProjectByZipOrFileService(ImportMainFrameProjectDTO request, List<IFormFile>? files);
        Task<GetMainFrameProjectDetailsByAuthTokenResponse> GetMainFrameProjectDetailsByAuthTokenService(string authToken);
        Task<EditMainFrameSourceProjectResponse> EditMainFrameSourceProjectService(EditMainFrameSourceProjectDTO request);
        Task<DeleteWorkspaceResponse> DeleteMainFrameSourceProjectService(int projectId);
        Task<GetMainFrameSourceProjectResponse> GetMainFrameSourceProjectService(int projectId);
        Task<GetMainFrameSourceProjectResponse> GetMainFrameDestinationProjectService(int projectId);
        Task<GetMainFrameProjectDetailsResponse> GetMainFrameProjectDetailsService(int itemId, string itemType, string projectType);
        Task<ImportResponse> CreateMainFrameChunkFileService(CreateMainFrameChunkFileDTO request);
        Task<ImportResponse> MoveMainFrameChunkFileService(int destinationFileId, string destinationFilePath);
        Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByZipService(int destinationProjectId);
        Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByURLService(ExportMainFrameProjectByURLServiceDTO exportMainFrameProjectByURLServiceDTO);
        Task<DeleteWorkspaceResponse> DeleteMainFrameDestinationFiles(List<int> filesId);
    }
}
