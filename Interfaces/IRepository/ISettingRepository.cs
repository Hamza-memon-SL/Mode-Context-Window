using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Request.Jira;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Response.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface ISettingRepository
    {
        Task<bool> CreateAboutAsync(AboutDTO aboutDTO);
        Task<AboutModel?> GetLatestAboutAsync();
        Task<bool> CreateDownloadAsync(DownloadsDto downloadsDto);
        Task<DownloadModel?> GetLatestDownloadAsync();
        Task<bool> UpdateDownloadAsync(UpdateDownloadDto downloadDto);
        Task<ExtensionBuild?> GetExtensionBuildByVersionAsync(string version);
        Task<bool> AddExtensionBuildAsync(ExtensionBuild extensionBuild);
        Task<List<ExtensionBuild>> GetPaginatedExtensionBuildsAsync(int pageNumber, int pageSize);
        Task<bool> CreateAnnouncementAsync(Announcement announcement);
        Task<PaginatedResult<AnnouncementDto>> GetPaginatedAnnouncementsAsync(int pageNumber, int pageSize);
        Task<bool> MarkAsLatestAsync(int id);
        Task<ExtensionBuild?> GetLatestMarkedAsDefaultBuildAsync();
        Task<int> GetLatestAnnouncementIdAsync(List<string> userGroups, int currentAnnouncementId);
        Task<bool> DeleteExtensionBuildAsync(int id);
        Task<bool> DeleteAnnouncementAsync(int announcementId);
        Task<Announcement> UpdateAnnouncementAsync(AnnouncementDto announcementDto);
        Task<Announcement> GetAnnouncementByIdAsync(int id);
        Task<CustomizationTimeSaved> GetAllEstimatedTimeSavedAsync();
        Task<bool> UpdateEstimatedTimeSavedAsync(UpdateEstimatedTimeSavedRequest request);
        Task<bool> UpdateExtensionBuildAsync(ExtensionBuild extensionBuild);
        Task<object> GetConfigurationsAsync(string moduleName);
        Task<bool> UpdateConfigurationsAsync(UpdateConfigurationsDTO dto);

    }
}
