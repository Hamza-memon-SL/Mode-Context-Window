using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Http;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface ISettingService
    {
        Task<Response<bool>> CreateAboutAsync(AboutDTO aboutDTO);
        Task<Response<AboutModel>> GetLatestAboutAsync();

        Task<Response<bool>> CreateDownloadAsync(DownloadsDto downloadsDto);
        Task<Response<DownloadModel>> GetLatestDownloadAsync();
        Task<Response<bool>> UpdateDownloadAsync(UpdateDownloadDto downloadDto);
        Task<Response<bool>> CreateExtensionBuildAsync(CreateExtensionBuildDTO createExtension);
        Task<ResponseList<ExtensionBuild>> GetPaginatedExtensionBuildsAsync(int pageNumber, int pageSize);

        Task<Response<bool>> UpdateExtensionBuildAsync(UpdateExtensionBuildDto extensionBuildDto);
        Task<Response<bool>> CreateAnnouncementAsync(CreateAnnouncementDto announcementDto);
        Task<PaginatedResponse<AnnouncementDto>> GetPaginatedAnnouncementsAsync(int pageNumber, int pageSize);
        Task<Response<bool>> MarkAsLatestAsync(int id);
        Task<Response<VisibotResponseDTO>> VisibotUponLoadAsync(VisibotRequestDTO request);

        Task<Response<bool>> DeleteExtensionBuildAsync(int id);
        Task<Core.Response.Response<bool>> DeleteAnnouncementAsync(int id);
        Task<Core.Response.Response<bool>> UpdateAnnouncementAsync(AnnouncementDto announcementDto);
        Task<Response<AnnouncementDto>> GetAnnouncementByIdAsync(int id);
        Task<GetAllEstimateTimeSaved> GetAllEstimatedTimeSaved();
        Task<UpdateEstimatedTimeSavedResponse> UpdateEstimatedTimeSaved(UpdateEstimatedTimeSavedRequest request);
        Task<Response<object>> GetConfigurationsService(string moduleName);
        Task<Core.Response.Response> UpdateConfigurationsService(UpdateConfigurationsDTO dto);

    }
}
