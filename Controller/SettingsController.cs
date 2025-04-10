using GenAiPoc.Application.Services;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Mvc;

namespace GenAiPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        [HttpPost("About")]
        public async Task<Response<bool>> CreateAboutAsync(AboutDTO aboutDTO)
        {
            var response = await _settingService.CreateAboutAsync(aboutDTO);
            return response;
        }

        [HttpGet("GetLatestAboutDetail")]
        public async Task<Response<AboutModel>> GetLatestAboutAsync()
        {
            var response = await _settingService.GetLatestAboutAsync();
            return response;
        }

        [HttpPost("Download")]
        public async Task<Response<bool>> CreateDownloadAsync([FromBody] DownloadsDto downloadsDto)
        {
            var response = await _settingService.CreateDownloadAsync(downloadsDto);
            return response;
        }

        [HttpGet("GetLatestDownload")]
        public async Task<Response<DownloadModel>> GetLatestDownloadAsync()
        {
            var response = await _settingService.GetLatestDownloadAsync();
            return response;
        }

        [HttpPost("UpdateDownload")]
        public async Task<Response<bool>> UpdateDownloadAsync(UpdateDownloadDto downloadDto)
        {
            var response = await _settingService.UpdateDownloadAsync(downloadDto);
            return response;
        }

        [HttpPost("CreateExtensionBuild")]
        public async Task<Response<bool>> CreateExtensionBuildAsync([FromForm] CreateExtensionBuildDTO createExtension)
        {
            var response = await _settingService.CreateExtensionBuildAsync(createExtension);
            return response;
        }

        [HttpGet("GetExtensionBuilds")]
        public async Task<ResponseList<ExtensionBuild>> GetPaginatedExtensionBuildsAsync(int pageNumber, int pageSize)
        {
            var response = await _settingService.GetPaginatedExtensionBuildsAsync(pageNumber, pageSize);
            return response;
        }

        [HttpPut("UpdateExtensionBuild")]
        public async Task<Response<bool>> UpdateExtensionBuildAsync([FromForm] UpdateExtensionBuildDto extensionBuildDto)
        {
            var response = await _settingService.UpdateExtensionBuildAsync(extensionBuildDto);
            return response;
        }

        [HttpDelete("DeleteExtensionBuild")]
        public async Task<Response<bool>> DeleteExtensionBuildAsync(int id)
        {
            var response = await _settingService.DeleteExtensionBuildAsync(id);
            return response;
        }

        [HttpPost("CreateAnnouncement")]
        public async Task<Response<bool>> CreateAnnouncementAsync([FromBody] CreateAnnouncementDto announcementDto)
        {
            var response = await _settingService.CreateAnnouncementAsync(announcementDto);
            return response;
        }

        [HttpGet("GetAnnouncements")]
        public async Task<PaginatedResponse<AnnouncementDto>> GetPaginatedAnnouncementsAsync(int pageNumber, int pageSize)
        {
            var response = await _settingService.GetPaginatedAnnouncementsAsync(pageNumber, pageSize);
            return response;
        }

        [HttpPut("UpdateAnnouncement")]
        public async Task<Response<bool>> UpdateAnnouncementAsync(AnnouncementDto announcementDto)
        {
            var response = await _settingService.UpdateAnnouncementAsync(announcementDto);
            return response;
        }


        [HttpDelete("DeleteAnnouncement")]
        public async Task<Response<bool>> DeleteAnnouncementAsync(int announcementId)
        {
            var response = await _settingService.DeleteAnnouncementAsync(announcementId);
            return response;
        }

        [HttpGet("MarkAsLatest")]
        public async Task<Response<bool>> MarkAsLatestAsync(int id)
        {
            var response = await _settingService.MarkAsLatestAsync(id);
            return response;
        }

        [HttpPost("VisibotUponLoad")]
        public async Task<Response<VisibotResponseDTO>> VisibotUponLoadAsync([FromBody] VisibotRequestDTO request)
        {
            var response = await _settingService.VisibotUponLoadAsync(request);
            return response;
        }

        [HttpGet("Announcement/{id}")]
        public async Task<Response<AnnouncementDto>> GetAnnouncementByIdAsync(int id)
        {
            var response = await _settingService.GetAnnouncementByIdAsync(id);
            return response;
        }
        [HttpGet("GetAllEstimatedTimeSaved")]
        public async Task<GetAllEstimateTimeSaved> GetAllEstimatedTimeSaved()
        {
            var response = await _settingService.GetAllEstimatedTimeSaved();
            return response;
        }
        [HttpPost("UpdateEstimatedTimeSaved")]
        public async Task<UpdateEstimatedTimeSavedResponse> UpdateEstimatedTimeSaved(UpdateEstimatedTimeSavedRequest request)
        {
            var response = await _settingService.UpdateEstimatedTimeSaved(request);
            return response;
        }
        [HttpGet("GetConfigurations")]
        public async Task<Response<Object>> GetConfigurations(string moduleName)
        {
            var response = await _settingService.GetConfigurationsService(moduleName);
            return response;
        }
        [HttpPost("UpdateConfigurations")]
        public async Task<Response> UpdateConfigurations(UpdateConfigurationsDTO dto)
        {
            var response = await _settingService.UpdateConfigurationsService(dto);
            return response;
        }

    }
}
