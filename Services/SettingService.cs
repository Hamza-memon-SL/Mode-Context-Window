using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Interfaces.IService.IVisionetClientService;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Infrastructure.Services;
using Microsoft.Graph.Models;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Request;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Results;

namespace GenAiPoc.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly ILogger<SettingService> _logger;
        private readonly ISettingRepository _settingRepository;
        private readonly IBlobStorageService _storageService;

        public SettingService(ILogger<SettingService> logger, ISettingRepository settingRepository, IBlobStorageService storageService)
        {
            _logger = logger;
            this._settingRepository = settingRepository;
            this._storageService = storageService;
        }

        public async Task<Response<bool>> CreateAboutAsync(AboutDTO aboutDTO)
        {
            Response<bool> response = new Response<bool>();
            try
            {

                _logger.LogInformation($"Starting creation process for About: {aboutDTO.Name}");

                byte[] data = Convert.FromBase64String(aboutDTO.Description!);
                string decodedDescription = Encoding.UTF8.GetString(data);
                aboutDTO.Description = decodedDescription;
                var creationResult = await _settingRepository.CreateAboutAsync(aboutDTO);

                if (!creationResult)
                {
                    _logger.LogWarning("Failed to create About record.");
                    response.Message = "Failed to create About record.";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating About.");
                response.Message = "An error occurred while creating About.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<AboutModel>> GetLatestAboutAsync()
        {
            Response<AboutModel> response = new Response<AboutModel>();
            try
            {
                _logger.LogInformation("Fetching the latest AboutModel item.");

                var latestAbout = await _settingRepository.GetLatestAboutAsync();

                if (latestAbout == null)
                {
                    _logger.LogWarning("No AboutModel item found.");
                    response.Message = "No item found.";
                    response.Success = false;
                    return response;
                }

                response.Data = latestAbout;
                response.Message = "item fetched successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the latest item.");
                response.Message = "An error occurred while fetching the data.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<bool>> CreateDownloadAsync(DownloadsDto downloadsDto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                byte[] data = Convert.FromBase64String(downloadsDto.Description!);
                string decodedDescription = Encoding.UTF8.GetString(data);
                downloadsDto.Description = decodedDescription;
                var isCreated = await _settingRepository.CreateDownloadAsync(downloadsDto);

                if (!isCreated)
                {
                    _logger.LogWarning($"Failed to create Download with Name: {downloadsDto?.Name}");
                    response.Message = $"Failed to create";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Item created successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Download.");
                response.Message = "An error occurred while creating Download.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<DownloadModel>> GetLatestDownloadAsync()
        {
            Response<DownloadModel> response = new Response<DownloadModel>();
            try
            {
                _logger.LogInformation("Fetching the latest download entry.");

                var latestDownload = await _settingRepository.GetLatestDownloadAsync();

                if (latestDownload == null)
                {
                    _logger.LogWarning("No download entries found.");
                    response.Message = "No entries found.";
                    response.Success = false;
                    return response;
                }

                response.Data = latestDownload;
                response.Message = "Operation completed successfully.";
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the latest download.");
                response.Message = "An error occurred while fetching the latest download.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<bool>> UpdateDownloadAsync(UpdateDownloadDto downloadDto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (string.IsNullOrEmpty(downloadDto.Description))
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;

                    return response;
                }

                byte[] data = Convert.FromBase64String(downloadDto.Description!);
                string decodedDescription = Encoding.UTF8.GetString(data);
                downloadDto.Description = decodedDescription;

                var updateSuccess = await _settingRepository.UpdateDownloadAsync(downloadDto);

                if (!updateSuccess)
                {
                    response.Message = $"No download found with ID: {downloadDto.Id}";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the download.");
                response.Message = "An error occurred while updating the download.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<bool>> CreateExtensionBuildAsync(CreateExtensionBuildDTO createExtension)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (createExtension.File == null || string.IsNullOrEmpty(createExtension.Version))
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                var existingBuild = await _settingRepository.GetExtensionBuildByVersionAsync(createExtension.Version);
                if (existingBuild != null)
                {
                    _logger.LogWarning("Version already exists: {Version}", createExtension.Version);
                    response.Message = "Version already exists.";
                    response.Success = false;
                    return response;
                }

                // builds is the container name
                var blobPath = Guid.NewGuid().ToString() + "_" + createExtension.File.FileName;
                var filePath = await _storageService.UploadFileToBlobAsync(createExtension.File, blobPath);

                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogError("File upload failed.");
                    response.Message = "File upload failed.";
                    response.Success = false;
                    return response;
                }

                _logger.LogInformation("Creating new ExtensionBuild record...");

                var newBuild = new ExtensionBuild
                {
                    Version = createExtension.Version,
                    Path = filePath,
                    Message = createExtension.Message,
                    MarkedAsLatest = false,
                    CreatedBy = createExtension.AuthToken,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                var isAdded = await _settingRepository.AddExtensionBuildAsync(newBuild);

                if (!isAdded)
                {
                    _logger.LogError("Failed to add ExtensionBuild to the database.");
                    response.Message = "Database operation failed.";
                    response.Success = false;
                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating ExtensionBuild.");
                response.Message = "An error occurred while creating ExtensionBuild.";
                response.Success = false;
                return response;
            }
        }

        public async Task<ResponseList<ExtensionBuild>> GetPaginatedExtensionBuildsAsync(int pageNumber, int pageSize)
        {
            ResponseList<ExtensionBuild> response = new ResponseList<ExtensionBuild>();
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }


                var paginatedBuilds = await _settingRepository.GetPaginatedExtensionBuildsAsync(pageNumber, pageSize);

                if (paginatedBuilds == null || !paginatedBuilds.Any())
                {
                    response.Message = "No Builds found.";
                    response.Success = false;
                    return response;
                }

                response.Data = paginatedBuilds;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated ExtensionBuilds.");
                response.Message = "An error occurred while fetching paginated ExtensionBuilds.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<bool>> CreateAnnouncementAsync(CreateAnnouncementDto announcementDto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (announcementDto == null || string.IsNullOrEmpty(announcementDto.Title) || string.IsNullOrEmpty(announcementDto.Description))
                {
                    _logger.LogWarning("Invalid input parameters: AnnouncementDto is null or required fields are missing.");
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                _logger.LogInformation("Starting announcement creation process.");

                byte[] data = Convert.FromBase64String(announcementDto.Description);
                string decodedDescription = Encoding.UTF8.GetString(data);

                var created = await _settingRepository.CreateAnnouncementAsync(new Announcement
                {
                    Title = announcementDto.Title,
                    Description = decodedDescription,
                    UserGroup = announcementDto.UserGroup,
                    CreatedBy = announcementDto.AuthToken,
                    CreatedDate = DateTime.UtcNow

                });

                if (!created)
                {
                    _logger.LogWarning("Failed to create the announcement.");
                    response.Message = "Failed to create the announcement.";
                    response.Success = false;
                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the announcement.");
                response.Message = "An error occurred while creating the announcement.";
                response.Success = false;

                return response;
            }
        }

        public async Task<PaginatedResponse<AnnouncementDto>> GetPaginatedAnnouncementsAsync(int pageNumber, int pageSize)
        {
            PaginatedResponse<AnnouncementDto> response = new PaginatedResponse<AnnouncementDto>();
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    _logger.LogWarning("Invalid input parameters: pageNumber or pageSize is less than or equal to zero.");
                    response.Message = "Invalid input parameters.";
                    response.Success = false;

                    return response;
                }
                var paginatedAnnouncements = await _settingRepository.GetPaginatedAnnouncementsAsync(pageNumber, pageSize);

                if (paginatedAnnouncements == null || !paginatedAnnouncements.Items.Any())
                {
                    response.Message = "No announcements found.";
                    response.Success = false;

                    return response;
                }

                response.Data = paginatedAnnouncements;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated announcements.");
                response.Message = "An error occurred while fetching announcements.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<bool>> MarkAsLatestAsync(int id)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid input parameter: id must be greater than 0.");
                    response.Message = "Invalid input parameter.";
                    response.Success = false;
                    return response;
                }

                var result = await _settingRepository.MarkAsLatestAsync(id);

                if (!result)
                {
                    response.Message = $"No record found for the provided ID: {id}.";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking ExtensionBuild as latest.");
                response.Message = "An error occurred while marking ExtensionBuild as latest.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<VisibotResponseDTO>> VisibotUponLoadAsync(VisibotRequestDTO request)
        {
            Response<VisibotResponseDTO> response = new Response<VisibotResponseDTO>();

            try
            {
                if (request == null || string.IsNullOrEmpty(request.ExtensionVersion) || !request.userGroups.Any())
                {
                    _logger.LogWarning("Invalid input parameters: request is null or missing required fields.");
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                var latestBuild = await _settingRepository.GetLatestMarkedAsDefaultBuildAsync();
                if (latestBuild == null)
                {
                    response.Message = "No default build found.";
                    response.Success = false;
                    return response;
                }

                if (latestBuild.Version == request.ExtensionVersion)
                {
                    // Section C: Check for new announcements
                    var latestAnnouncementId = await _settingRepository.GetLatestAnnouncementIdAsync(request.userGroups, request.AnnouncementId);
                    response.Data = new VisibotResponseDTO
                    {
                        Message = string.Empty,
                        Path = string.Empty,
                        AnnouncementId = latestAnnouncementId
                    };
                }
                else
                {
                    // Section B: Return updated build information
                    response.Data = new VisibotResponseDTO
                    {
                        Message = latestBuild.Message,
                        Path = latestBuild.Path,
                        AnnouncementId = 0
                    };
                }

                response.Message = "Operation completed successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing VisibotUponLoad.");
                response.Message = "An error occurred while processing the request.";
                response.Success = false;
            }

            return response;
        }

        public async Task<Response<bool>> DeleteExtensionBuildAsync(int id)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (id <= 0)
                {
                    response.Message = "Invalid input parameter.";
                    response.Success = false;

                    return response;
                }


                var result = await _settingRepository.DeleteExtensionBuildAsync(id);

                if (!result)
                {
                    response.Message = $"No ExtensionBuild found for deletion with Id: {id}";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting ExtensionBuild.");
                response.Message = "An error occurred while deleting ExtensionBuild.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<bool>> DeleteAnnouncementAsync(int announcementId)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                if (announcementId <= 0)
                {
                    _logger.LogWarning("Invalid input parameters: announcementId is null or empty.");
                    response.Message = "Invalid input parameters.";
                    response.Success = false;

                    return response;
                }

                var announcementToDelete = await _settingRepository.DeleteAnnouncementAsync(announcementId);

                if (!announcementToDelete)
                {
                    _logger.LogWarning($"No announcement found for deletion with AnnouncementId: {announcementId}");
                    response.Message = $"No announcement found for deletion with AnnouncementId: {announcementId}";
                    response.Success = false;

                    return response;
                }
                response.Data = true;
                response.Message = $"Operation completed successfully.";
                response.Success = true;

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the announcement.");
                response.Message = $"An error occurred while deleting the announcement.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<bool>> UpdateAnnouncementAsync(AnnouncementDto announcementDto)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                if (announcementDto == null)
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;

                    return response;
                }
                byte[] data = Convert.FromBase64String(announcementDto.Description!);
                string decodedDescription = Encoding.UTF8.GetString(data);
                announcementDto.Description = decodedDescription;

                var updatedAnnouncement = await _settingRepository.UpdateAnnouncementAsync(announcementDto);

                if (updatedAnnouncement == null)
                {
                    response.Message = $"Announcement not found with Id: {announcementDto.Id}";
                    response.Success = false;

                    return response;
                }

                response.Data = true;
                response.Message = "Announcement updated successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the announcement.");
                response.Message = "An error occurred while updating the announcement.";
                response.Success = false;

                return response;
            }
        }

        public async Task<Response<AnnouncementDto>> GetAnnouncementByIdAsync(int id)
        {
            Response<AnnouncementDto> response = new Response<AnnouncementDto>();
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid ID provided.");
                    response.Message = "Invalid ID.";
                    response.Success = false;

                    return response;
                }


                var announcement = await _settingRepository.GetAnnouncementByIdAsync(id);

                if (announcement == null)
                {
                    response.Message = $"No announcement found with ID: {id}";
                    response.Success = false;

                    return response;
                }

                response.Data = new AnnouncementDto
                {
                    Title = announcement.Title,
                    Description = announcement.Description,
                    UserGroup = announcement.UserGroup,
                    CreatedDate = announcement.CreatedDate,
                    Id = announcement.Id,
                };
                response.Message = "Announcement fetched successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the announcement.");
                response.Message = "An error occurred while fetching the announcement.";
                response.Success = false;

                return response;
            }
        }
        public async Task<GetAllEstimateTimeSaved> GetAllEstimatedTimeSaved()
        {
            try
            {
                var result = await _settingRepository.GetAllEstimatedTimeSavedAsync();
                if (result != null)
                {
                    return new GetAllEstimateTimeSaved(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, result);
                }
                else
                {
                    return new GetAllEstimateTimeSaved(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, null);
                }
            }
            catch (Exception)
            {
                return new GetAllEstimateTimeSaved(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<UpdateEstimatedTimeSavedResponse> UpdateEstimatedTimeSaved(UpdateEstimatedTimeSavedRequest request)
        {
            try
            {
                var result = await _settingRepository.UpdateEstimatedTimeSavedAsync(request);
                if (result)
                {
                    return new UpdateEstimatedTimeSavedResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage);
                }
                else
                {
                    return new UpdateEstimatedTimeSavedResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.ErrorMessage);
                }
            }
            catch (Exception)
            {
                return new UpdateEstimatedTimeSavedResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong);
            }
        }

        public async Task<Response<bool>> UpdateExtensionBuildAsync(UpdateExtensionBuildDto extensionBuildDto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (extensionBuildDto == null)
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                ExtensionBuild extensionBuild = new ExtensionBuild
                {
                    Id = extensionBuildDto.Id,
                    Message = extensionBuildDto.Message,
                    Version = extensionBuildDto.Version,
                };

                // builds is the container name
                if (extensionBuildDto.File != null)
                {

                    var fileName = Guid.NewGuid().ToString() + ".vsix";
                    var filePath = await _storageService.UploadFileToBlobAsync(extensionBuildDto.File, fileName);
                    extensionBuild.Path = filePath;
                }

                var isUpdated = await _settingRepository.UpdateExtensionBuildAsync(extensionBuild);

                if (!isUpdated)
                {
                    response.Message = $"No ExtensionBuild found with ID: {extensionBuildDto.Id}";
                    response.Success = false;
                    return response;
                }

                response.Data = true;
                response.Message = "Operation completed successfully.";
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ExtensionBuild.");
                response.Message = "An error occurred while updating the ExtensionBuild.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<object>> GetConfigurationsService(string moduleName)
        {
            try
            {
                var result = await _settingRepository.GetConfigurationsAsync(moduleName);
                if (result != null)
                {
                    return new Response<object> { Success = StatusAndMessagesKeys.SuccessStatus == 0, Message = StatusAndMessagesKeys.SuccessMessage, Data = result };
                }
                else
                {
                    return new Response<object> { Success = StatusAndMessagesKeys.ErrorStatus == 1, Message = StatusAndMessagesKeys.ErrorMessage, Data = result };
                }
            }
            catch (Exception)
            {
                return new Response<object> { Success = StatusAndMessagesKeys.ErrorStatus == 1, Message = StatusAndMessagesKeys.SomethingWentWrong };
            }
        }
        public async Task<Response> UpdateConfigurationsService(UpdateConfigurationsDTO dto)
        {
            try
            {
                var result = await _settingRepository.UpdateConfigurationsAsync(dto);
                if (result != null)
                {
                    return new Response { Success = StatusAndMessagesKeys.SuccessStatus == 0, Message = StatusAndMessagesKeys.SuccessMessage };
                }
                else
                {
                    return new Response { Success = StatusAndMessagesKeys.ErrorStatus == 1, Message = StatusAndMessagesKeys.ErrorMessage };
                }
            }
            catch (Exception)
            {
                return new Response { Success = StatusAndMessagesKeys.ErrorStatus == 1, Message = StatusAndMessagesKeys.SomethingWentWrong };
            }
        }
    }
}
