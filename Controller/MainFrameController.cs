using AutoMapper;
using GenAiPoc.Application.Services;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GenAiPoc.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MainFrameController : Controller
    {
        private readonly IMainFrameService _mainFrameService;
        private readonly IAIModelService _aiModelService;
        private readonly IMapper _mapper;
        public MainFrameController(IMainFrameService mainFrameService, IMapper mapper, IAIModelService aiModelService)
        {
            _mainFrameService = mainFrameService;
            _mapper = mapper;
            _aiModelService = aiModelService;
        }

        [HttpPost("ImportMainFrameProject")]
        public async Task<ImportResponse> ImportMainFrameProject([FromForm] ImportMainFrameProjectDTO request)
        {
            if (request.files != null && request.files.Count > 0)
            {
                var response = await _mainFrameService.ImportMainFrameProjectByZipOrFileService(request, request.files);
                return response;
            }
            else
            {
                var response = await _mainFrameService.ImportMainFrameProjectByURLService(request);
                return response;
            }
        }

        [HttpGet("GetMainFrameProjectDetailsByAuthToken")]
        public async Task<GetMainFrameProjectDetailsByAuthTokenResponse> GetMainFrameProjectDetailsByAuthToken(string authToken)
        {
            var response = await _mainFrameService.GetMainFrameProjectDetailsByAuthTokenService(authToken);
            return response;
        }

        [HttpPost("EditMainFrameSourceProject")]
        public async Task<EditMainFrameSourceProjectResponse> EditMainFrameSourceProject(EditMainFrameSourceProjectDTO request)
        {
            var response = await _mainFrameService.EditMainFrameSourceProjectService(request);
            return response;
        }
        [HttpPost("DeleteMainFrameSourceProject")]
        public async Task<DeleteWorkspaceResponse> DeleteMainFrameSourceProject(int projectId)
        {
            var response = await _mainFrameService.DeleteMainFrameSourceProjectService(projectId);
            return response;
        }
        [HttpGet("GetMainFrameProjectDetails")]
        public async Task<GetMainFrameProjectDetailsResponse> GetMainFrameProjectDetails(int itemId, string itemType, string projectType)
        {
            var response = await _mainFrameService.GetMainFrameProjectDetailsService(itemId, itemType, projectType);
            return response;
        }
        [HttpGet("GetMainFrameSourceProject")]
        public async Task<GetMainFrameSourceProjectResponse> GetMainFrameSourceProject(int projectId)
        {
            var response = await _mainFrameService.GetMainFrameSourceProjectService(projectId);
            return response;
        }
        [HttpGet("GetMainFrameDestinationProject")]
        public async Task<GetMainFrameSourceProjectResponse> GetMainFrameDestinationProject(int projectId)
        {
            var response = await _mainFrameService.GetMainFrameDestinationProjectService(projectId);
            return response;
        }
        [HttpPost("CreateMainFrameChunkFile")]
        public async Task<ImportResponse> CreateMainFrameChunkFile([FromForm] CreateMainFrameChunkFileDTO request)
        {
            if (request.MainFrameDestinationFile != null)
            {
                var response = await _mainFrameService.CreateMainFrameChunkFileService(request);
                return response;
            }
            else
            {
                return new ImportResponse(0, "No file provided.");
            }
        }
        [HttpPost("MoveMainFrameChunkFile")]
        public async Task<ImportResponse> MoveMainFrameChunkFile(int destinationFileId, string destinationFilePath)
        {
            var response = await _mainFrameService.MoveMainFrameChunkFileService(destinationFileId, destinationFilePath);
            return response;
        }
        [HttpPost("ExportMainFrameProjectByZip")]
        public async Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByZip(int destinationProjectId)
        {
            var response = await _mainFrameService.ExportMainFrameProjectByZipService(destinationProjectId);
            return response;
        }
        [HttpPost("ExportMainFrameProjectByURL")]
        public async Task<ExportMainFrameProjectByZipResponse> ExportMainFrameProjectByURL(ExportMainFrameProjectByURLServiceDTO exportMainFrameProjectByURLServiceDTO)
        {
            var response = await _mainFrameService.ExportMainFrameProjectByURLService(exportMainFrameProjectByURLServiceDTO);
            return response;
        }
        [HttpPost("DeleteMainFrameDestinationFiles")]
        public async Task<DeleteWorkspaceResponse> DeleteMainFrameDestinationFiles(List<int> filesId)
        {
            var response = await _mainFrameService.DeleteMainFrameDestinationFiles(filesId);
            return response;
        }
    }
}