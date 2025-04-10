using EFCore.BulkExtensions;
using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Infrastructure.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private readonly DbContextGenAiPOC _dbContextGenAiPOC;

        public SettingRepository(DbContextGenAiPOC dbContextGenAiPOC)
        {
            this._dbContextGenAiPOC = dbContextGenAiPOC;
        }

        public async Task<bool> CreateAboutAsync(AboutDTO aboutDTO)
        {
            try
            {
                var aboutEntity = new AboutModel
                {
                    Name = aboutDTO.Name,
                    Description = aboutDTO.Description,
                    Version = aboutDTO.Version,
                    CreatedBy = aboutDTO.CreatedBy
                };

                await _dbContextGenAiPOC.AboutModels.AddAsync(aboutEntity);
                await _dbContextGenAiPOC.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating About: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateDownloadAsync(DownloadsDto downloadsDto)
        {
            try
            {
                var download = new DownloadModel
                {
                    Description = downloadsDto.Description,
                    Name = downloadsDto.Name,
                    Version = downloadsDto.Version,
                    Path = downloadsDto.Path,
                    CreatedBy = downloadsDto.AuthToken
                };

                await _dbContextGenAiPOC.DownloadModels.AddAsync(download);
                await _dbContextGenAiPOC.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating download: {ex.Message}");
                return false;
            }
        }

        public async Task<ExtensionBuild?> GetExtensionBuildByVersionAsync(string version)
        {
            try
            {
                return await _dbContextGenAiPOC.ExtensionBuilds.FirstOrDefaultAsync(eb => eb.Version == version);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AboutModel?> GetLatestAboutAsync()
        {
            try
            {
                return await _dbContextGenAiPOC.AboutModels
                    .OrderByDescending(a => a.CreatedDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching the latest AboutModel item: {ex.Message}");
                return null;
            }
        }

        public async Task<DownloadModel?> GetLatestDownloadAsync()
        {
            try
            {
                return await _dbContextGenAiPOC.DownloadModels
                    .OrderByDescending(d => d.CreatedDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching the latest download: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateDownloadAsync(UpdateDownloadDto downloadDto)
        {
            try
            {
                var download = await _dbContextGenAiPOC.DownloadModels.FirstOrDefaultAsync(d => d.Id == downloadDto.Id);

                if (download == null)
                {
                    return false;
                }

                download.Description = downloadDto.Description;
                _dbContextGenAiPOC.DownloadModels.Update(download);
                await _dbContextGenAiPOC.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating download: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddExtensionBuildAsync(ExtensionBuild extensionBuild)
        {
            try
            {
                await _dbContextGenAiPOC.ExtensionBuilds.AddAsync(extensionBuild);
                await _dbContextGenAiPOC.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ExtensionBuild>> GetPaginatedExtensionBuildsAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _dbContextGenAiPOC.ExtensionBuilds
                    .OrderByDescending(b => b.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new ExtensionBuild
                    {
                        Id = b.Id,
                        Version = b.Version,
                        Path = b.Path,
                        Message = b.Message,
                        MarkedAsLatest = b.MarkedAsLatest,
                        CreatedDate = b.CreatedDate,
                        CreatedBy = b.CreatedBy
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paginated ExtensionBuilds: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> CreateAnnouncementAsync(Announcement announcement)
        {
            try
            {
                await _dbContextGenAiPOC.Announcements.AddAsync(announcement);
                await _dbContextGenAiPOC.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating announcement: {ex.Message}");
                return false;
            }
        }

        public async Task<PaginatedResult<AnnouncementDto>> GetPaginatedAnnouncementsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _dbContextGenAiPOC.Announcements
                    .OrderByDescending(a => a.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AnnouncementDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        UserGroup = a.UserGroup,
                        CreatedDate = a.CreatedDate
                    });

                var totalItems = await _dbContextGenAiPOC.Announcements.CountAsync();
                var items = await query.ToListAsync();

                return new PaginatedResult<AnnouncementDto>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paginated announcements: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> MarkAsLatestAsync(int id)
        {
            try
            {
                var allRecords = await _dbContextGenAiPOC.ExtensionBuilds
                    .ToListAsync();

                if (allRecords == null || !allRecords.Any())
                {
                    return false;
                }

                allRecords.ForEach(x =>
                {
                    x.MarkedAsLatest = false;
                });

                var targetRecord = allRecords.FirstOrDefault(r => r.Id == id);
                if (targetRecord != null)
                {
                    targetRecord.MarkedAsLatest = true;
                    await _dbContextGenAiPOC.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking ExtensionBuild as latest: {ex.Message}");
                return false;
            }
        }

        public async Task<ExtensionBuild?> GetLatestMarkedAsDefaultBuildAsync()
        {
            try
            {
                return await _dbContextGenAiPOC.ExtensionBuilds
                    .Where(eb => eb.MarkedAsLatest == true)
                    .OrderByDescending(eb => eb.CreatedDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching latest marked as default build: {ex.Message}");
                return null;
            }
        }

        public async Task<int> GetLatestAnnouncementIdAsync(List<string> userGroups, int currentAnnouncementId)
        {
            try
            {
                var announcement = await _dbContextGenAiPOC.Announcements
                    .Where(a => userGroups.Contains(a.UserGroup!) && a.Id > currentAnnouncementId)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefaultAsync();

                return announcement?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching latest announcement ID: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> DeleteExtensionBuildAsync(int id)
        {
            try
            {
                var extensionBuild = await _dbContextGenAiPOC.ExtensionBuilds
                    .FirstOrDefaultAsync(eb => eb.Id == id);

                if (extensionBuild != null)
                {
                    _dbContextGenAiPOC.ExtensionBuilds.Remove(extensionBuild);
                    await _dbContextGenAiPOC.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting ExtensionBuild: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAnnouncementAsync(int announcementId)
        {
            try
            {
                var announcementToDelete = await _dbContextGenAiPOC.Announcements
                    .Where(a => a.Id == announcementId)
                    .FirstOrDefaultAsync();

                if (announcementToDelete != null)
                {
                    _dbContextGenAiPOC.Announcements.Remove(announcementToDelete);
                    await _dbContextGenAiPOC.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting announcement: {ex.Message}");
                return false;
            }
        }

        public async Task<Announcement> UpdateAnnouncementAsync(AnnouncementDto announcementDto)
        {
            try
            {
                var announcementToUpdate = await _dbContextGenAiPOC.Announcements
                    .Where(a => a.Id == announcementDto.Id)
                    .FirstOrDefaultAsync();

                if (announcementToUpdate != null)
                {
                    announcementToUpdate.Title = announcementDto.Title ?? announcementToUpdate.Title;
                    announcementToUpdate.Description = announcementDto.Description ?? announcementToUpdate.Description;
                    announcementToUpdate.UserGroup = announcementDto.UserGroup ?? announcementToUpdate.UserGroup;

                    _dbContextGenAiPOC.Announcements.Update(announcementToUpdate);
                    await _dbContextGenAiPOC.SaveChangesAsync();
                    return announcementToUpdate;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating announcement: {ex.Message}");
                return null;
            }
        }

        public async Task<Announcement> GetAnnouncementByIdAsync(int id)
        {
            try
            {
                var announcement = await _dbContextGenAiPOC.Announcements
                    .FirstOrDefaultAsync(a => a.Id == id);
                return announcement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching announcement: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomizationTimeSaved> GetAllEstimatedTimeSavedAsync()
        {
            try
            {
                var result = await _dbContextGenAiPOC.CustomizationTimeSaved.Where(x => x.IsActive == true).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> UpdateEstimatedTimeSavedAsync(UpdateEstimatedTimeSavedRequest request)
        {
            bool isSuccess = false;
            try
            {
                var result = await _dbContextGenAiPOC.CustomizationTimeSaved.Where(x => x.Id.Equals(request.Id)).FirstOrDefaultAsync();
                if (result != null)
                {
                    result.EstimatedTimeSavedFormulaValue = request.EstimatedTimeSavedFormulaValue;
                    result.EstimatedTimeSavedDashboardFormat = request.EstimatedTimeSavedDashboardFormat;
                    result.EstimatedTimeSavedVisibotFormat = request.EstimatedTimeSavedVisibotFormat;

                    int isUpdated = await _dbContextGenAiPOC.SaveChangesAsync();
                    if (isUpdated > 0)
                    {
                        return isSuccess = true;
                    }
                    else
                    {
                        return isSuccess;
                    }
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateExtensionBuildAsync(ExtensionBuild extensionBuild)
        {
            try
            {
                var existingBuildDb = await _dbContextGenAiPOC
                    .ExtensionBuilds.FirstOrDefaultAsync(b => b.Id == extensionBuild.Id);

                if (existingBuildDb == null)
                {
                    return false;
                }

                existingBuildDb.Version = extensionBuild.Version;
                existingBuildDb.Message = extensionBuild.Message;
                existingBuildDb.Path = string.IsNullOrEmpty(extensionBuild.Path) ? existingBuildDb.Path : extensionBuild.Path;
                existingBuildDb.ModifiedDate = DateTime.UtcNow;

                await _dbContextGenAiPOC.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ExtensionBuild: {ex.Message}");
                return false;
            }
        }
        public async Task<object> GetConfigurationsAsync(string moduleName)
        {
            try
            {

                object result;

                if (moduleName == "codebuddy")
                {
                    result = await _dbContextGenAiPOC.Configurations.Select(c => new Configurations
                    {
                        SingleFileSummaryPrompt = c.SingleFileSummaryPrompt,
                        ProjectFileSummaryPrompt = c.ProjectFileSummaryPrompt,
                        SingleFileSummaryTemplate = c.SingleFileSummaryTemplate,
                        ProjectFileSummaryTemplate = c.ProjectFileSummaryTemplate,
                        JsonValidationRetryAttempt = c.JsonValidationRetryAttempt,
                    }).FirstOrDefaultAsync();
                }
                else if (moduleName == "backlogbuddy")
                {
                    result = await _dbContextGenAiPOC.Configurations.Select(c => new Configurations
                    {
                        NonConformancePrompt = c.NonConformancePrompt,
                        AmbiguousPrompt = c.AmbiguousPrompt,
                        SemanticPrompt = c.SemanticPrompt,

                    }).FirstOrDefaultAsync();
                }
                else
                {
                    result = await _dbContextGenAiPOC.MainFrameConfigurations.Select(c => new MainFrameConfigurations
                    {
                        SimpleSummarizeCodePrompt = c.SimpleSummarizeCodePrompt,
                        SingleFileSummarizePrompt = c.SingleFileSummarizePrompt,
                        CodeSplitSuggestionPrompt = c.CodeSplitSuggestionPrompt,

                    }).FirstOrDefaultAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<bool> UpdateConfigurationsAsync(UpdateConfigurationsDTO dto)
        {
            try
            {
                if (dto.ModuleName == "codebuddy")
                {
                    var existingConfig = await _dbContextGenAiPOC.Configurations.FirstOrDefaultAsync();
                    if (existingConfig != null)
                    {
                        if (!string.IsNullOrEmpty(dto.UpdatedConfig.SingleFileSummaryPrompt))
                            existingConfig.SingleFileSummaryPrompt = dto.UpdatedConfig.SingleFileSummaryPrompt;

                        if (!string.IsNullOrEmpty(dto.UpdatedConfig.ProjectFileSummaryPrompt))
                            existingConfig.ProjectFileSummaryPrompt = dto.UpdatedConfig.ProjectFileSummaryPrompt;

                        if (!string.IsNullOrEmpty(dto.UpdatedConfig.SingleFileSummaryTemplate))
                            existingConfig.SingleFileSummaryTemplate = dto.UpdatedConfig.SingleFileSummaryTemplate;

                        if (!string.IsNullOrEmpty(dto.UpdatedConfig.ProjectFileSummaryTemplate))
                            existingConfig.ProjectFileSummaryTemplate = dto.UpdatedConfig.ProjectFileSummaryTemplate;

                        if (dto.UpdatedConfig.JsonValidationRetryAttempt != null)
                            existingConfig.JsonValidationRetryAttempt = dto.UpdatedConfig.JsonValidationRetryAttempt;

                        _dbContextGenAiPOC.Configurations.Update(existingConfig);
                    }
                }
                else if (dto.ModuleName == "backlogbuddy")
                {
                    var existingConfig = await _dbContextGenAiPOC.Configurations.FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(dto.UpdatedConfig.NonConformancePrompt))
                        existingConfig.NonConformancePrompt = dto.UpdatedConfig.NonConformancePrompt;

                    if (!string.IsNullOrEmpty(dto.UpdatedConfig.AmbiguousPrompt))
                        existingConfig.AmbiguousPrompt = dto.UpdatedConfig.AmbiguousPrompt;

                    if (!string.IsNullOrEmpty(dto.UpdatedConfig.SemanticPrompt))
                        existingConfig.SemanticPrompt = dto.UpdatedConfig.SemanticPrompt;

                    _dbContextGenAiPOC.Configurations.Update(existingConfig);
                }
                else
                {
                    var existingMainFrameConfig = await _dbContextGenAiPOC.MainFrameConfigurations.FirstOrDefaultAsync();
                    if (existingMainFrameConfig != null)
                    {
                        if (!string.IsNullOrEmpty(dto.UpdatedMainFrameConfig.SimpleSummarizeCodePrompt))
                            existingMainFrameConfig.SimpleSummarizeCodePrompt = dto.UpdatedMainFrameConfig.SimpleSummarizeCodePrompt;

                        if (!string.IsNullOrEmpty(dto.UpdatedMainFrameConfig.SingleFileSummarizePrompt))
                            existingMainFrameConfig.SingleFileSummarizePrompt = dto.UpdatedMainFrameConfig.SingleFileSummarizePrompt;

                        if (!string.IsNullOrEmpty(dto.UpdatedMainFrameConfig.CodeSplitSuggestionPrompt))
                            existingMainFrameConfig.CodeSplitSuggestionPrompt = dto.UpdatedMainFrameConfig.CodeSplitSuggestionPrompt;

                        _dbContextGenAiPOC.MainFrameConfigurations.Update(existingMainFrameConfig);
                    }
                }

                await _dbContextGenAiPOC.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating configurations.", ex);
            }
        }
    }
}
