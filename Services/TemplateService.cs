using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using GenAiPoc.Core.Specification;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Application.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserTemplateRepository _userTemplateRepository;
        private readonly IBlobStorageService _blobStorage;
        private readonly IAuthenticationService _authenticationService;

        public TemplateService(ILogger<TemplateService> logger, 
            ITemplateRepository templateRepository,
            IUserTemplateRepository userTemplateRepository,
            IBlobStorageService blobStorage,
            IAuthenticationService authenticationService
            )
        {
            _logger = logger;
            this._templateRepository = templateRepository;
            this._userTemplateRepository = userTemplateRepository;
            this._blobStorage = blobStorage;
            this._authenticationService = authenticationService;
        }


        public async Task<Response<bool>> CreateTemplateCategoryAsync(CreateTemplateCategoryDto dto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                {
                    _logger.LogWarning("Invalid input: Name is null or empty.");
                    response.Message = "Invalid input: Name is required.";
                    response.Success = false;
                    return response;
                }

                var templateCategory = new TemplateCategory { Name = dto.Name };
                await _templateRepository.CreateTemplateCategoryAsync(templateCategory);

                response.Data = true;
                response.Message = "Template category created successfully.";
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a template category.");
                response.Message = "An error occurred while creating a template category.";
                response.Success = false;
                return response;
            }
        }

        public async Task<ResponseList<TemplateCategoryDto>> GetAllTemplateCategoryAsync()
        {
            ResponseList<TemplateCategoryDto> response = new ResponseList<TemplateCategoryDto>();
            try
            {
                var categories = await _templateRepository.GetAllTemplateCategoryAsync();
                response.Data = categories.Select(c => new TemplateCategoryDto { Id = c.Id, Name = c.Name }).ToList();
                response.Message = "Operation completed successfully.";
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving template categories.");
                response.Message = "An error occurred while retrieving template categories.";
                response.Success = false;
                return response;
            }
        }




        // Start of Template APIs
        public async Task<Response<bool>> CreateTemplateAsync(CreateTemplateDto templateDto)
        {
            var response = new Response<bool>();
            try
            {
                var isAdmin = await _authenticationService.IsAdminAsync(templateDto.AuthToken);
                var template = new Template
                {
                    Name = templateDto.Name,
                    Description = templateDto.Description,
                    TemplateCategoryId = templateDto.TemplateCategoryId,
                    Language = templateDto.Language,
                    CreatedDate = DateTime.UtcNow,
                    IsCreatedByAdmin = isAdmin,
                    CreatedBy = templateDto.CreatedBy,
                    AuthToken = templateDto.AuthToken

                };

                var result = await _templateRepository.CreateTemplateAsync(template);
                response.Data = result;
                response.Success = true;
                response.Message = "Template created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template.");
                response.Success = false;
                response.Message = "An error occurred while creating the template.";
            }
            return response;
        }

        public async Task<ResponseList<TemplateDto>> GetAllTemplatesAsync(int page, int pageSize, string? searchTerm)
        {
            var response = new ResponseList<TemplateDto>();
            try
            {
                var templates = await _templateRepository.GetAllTemplatesAsync(page, pageSize, searchTerm);
                response.Data = templates.Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    TemplateCategoryId = t.TemplateCategoryId,
                    Language = t.Language,
                    IsCreatedByAdmin = t.IsCreatedByAdmin,
                    CreatedDate = t.CreatedDate
                    
                }).ToList();
                response.Success = true;
                response.Message = response.Data != null ? StatusAndMessagesKeys.GetAllSuccess : StatusAndMessagesKeys.GetAllNoData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching templates.");
                response.Success = false;
                response.Message = "An error occurred while retrieving templates.";
            }
            return response;
        }

        public async Task<Response<TemplateDto>> GetTemplateByIdAsync(int id)
        {
            var response = new Response<TemplateDto>();
            try
            {
                var template = await _templateRepository.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Template not found.";
                    return response;
                }
                response.Data = new TemplateDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    TemplateCategoryId = template.TemplateCategoryId,
                    Language = template.Language,
                    IsCreatedByAdmin = template.IsCreatedByAdmin,
                    CreatedDate = template.CreatedDate,
                };
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching template by ID.");
                response.Success = false;
                response.Message = "An error occurred while retrieving the template.";
            }
            return response;
        }

        public async Task<ResponseList<TemplateDto>> GetTemplatesByCategoryIdAsync(int categoryId)
        {
            var response = new ResponseList<TemplateDto>();
            try
            {
                var templates = await _templateRepository.GetTemplatesByCategoryIdAsync(categoryId);
                response.Data = templates.Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    TemplateCategoryId = t.TemplateCategoryId,
                    Language = t.Language,
                    IsCreatedByAdmin = t.IsCreatedByAdmin,
                    CreatedDate = t.CreatedDate,
                }).ToList();
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching templates by category ID.");
                response.Success = false;
                response.Message = "An error occurred while retrieving templates.";
            }
            return response;
        }

        public async Task<Core.Response.Response<bool>> UpdateTemplateAsync(UpdateTemplateDto templateDto)
        {
            Core.Response.Response<bool> response = new Core.Response.Response<bool>();
            try
            {
                string imagePath = templateDto.ImagePath!;
                if (templateDto == null || templateDto.TemplateId <= 0)
                {
                    response.Message = "Invalid input parameters.";
                    response.Success = false;
                    return response;
                }

                if (templateDto.Image != null)
                {
                    var blobPath = Guid.NewGuid().ToString() + "_" + templateDto.Image.FileName;
                    imagePath = await _blobStorage.UploadTemplateImageToBlobAsync(templateDto.Image, blobPath);
                }
                templateDto.ImagePath = imagePath;
              //  templateDto.IsCreatedByAdmin = await _authenticationService.IsAdminAsync(templateDto.AuthToken);
                var updateResult = await _templateRepository.UpdateTemplateAsync(templateDto);
                response.Data = updateResult;
                response.Message = updateResult ? StatusAndMessagesKeys.UpdateSuccess : StatusAndMessagesKeys.GetByIdNotFound;
                response.Success = updateResult;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the template.");
                response.Message = "An error occurred while updating the template.";
                response.Success = false;
                return response;
            }
        }

        //End of Template APIs


        // Start of Template Section APIs

        public async Task<Response<bool>> CreateTemplateSectionAsync(TemplateSectionDTO templateSectionDto)
        {
            var entity = new TemplateSection
            {
                //TemplateId = templateSectionDto.TemplateId,
                Title = templateSectionDto.Title,
                Priority = templateSectionDto.Priority,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _templateRepository.CreateTemplateSectionAsync(entity);
            return new Response<bool> { Data = result, Success = result, Message = result ? "Created successfully" : "Creation failed" };
        }

        public async Task<ResponseList<TemplateSectionDTO>> GetAllTemplateSectionByTemplateIdAsync(int templateId)
        {
            var sections = await _templateRepository.GetAllTemplateSectionByTemplateIdAsync(templateId);
            var result = sections.ConvertAll(s => new TemplateSectionDTO { Id = s.Id, Title = s.Title, Priority = s.Priority });
            return new ResponseList<TemplateSectionDTO> { Data = result, Success = true, Message = "Fetched successfully" };
        }

        public async Task<Response<bool>> UpdateTemplateSectionAsync(TemplateSectionDTO templateSectionDto)
        {
            var entity = await _templateRepository.GetTemplateSectionByIdAsync(templateSectionDto.Id);
            if (entity == null)
                return new Response<bool> { Success = false, Message = "Not found" };

            entity.Title = templateSectionDto.Title;
            entity.Priority = templateSectionDto.Priority;
            entity.ModifiedDate = DateTime.UtcNow;

            var result = await _templateRepository.UpdateTemplateSectionAsync(entity);
            return new Response<bool> { Data = result, Success = result, Message = result ? "Updated successfully" : "Update failed" };
        }

        public async Task<Response<bool>> DeleteTemplateSectionAsync(int id)
        {
            var result = await _templateRepository.DeleteTemplateSectionAsync(id);
            return new Response<bool> { Data = result, Success = result, Message = result ? "Deleted successfully" : "Deletion failed" };
        }

        // end of Template APIs


        public async Task<Response<bool>> CreateUserTemplateAsync(CreateUserTemplateDto dto)
        {
            List<UserTemplate> userTemplates = new List<UserTemplate>();

            // Ensure TemplateSubSectionId is not null before iterating
            if (dto.TemplateSubSectionIds != null && dto.TemplateSubSectionIds.Any())
            {
                string imagePath = null;
                if (dto.Image != null)
                {
                    var blobPath = Guid.NewGuid().ToString() + "_" + dto.Image.FileName;
                    imagePath = await _blobStorage.UploadTemplateImageToBlobAsync(dto.Image, blobPath);
                }
                var isAdmin = await _authenticationService.IsAdminAsync(dto.CreatedBy);
                Template template = new Template
                {
                    CreatedBy = dto.CreatedBy,
                    AuthToken = dto.AuthToken,
                    CreatedDate = DateTime.UtcNow,
                    Description = dto.Description,
                    Name = dto.Title,
                    Language = dto.Language,
                    IsCreatedByAdmin = isAdmin,
                    TemplateCategoryId = dto.TemplateCategoryId,
                    ImagePath = imagePath
                };

                dto.TemplateSubSectionIds.ForEach(x =>
                {
                    userTemplates.Add(new UserTemplate
                    {
                        Template = template,
                        TemplateSubSectionId = x.Id,
                        SubSectionOrder = x.Order,
                        CreatedBy = dto.CreatedBy,
                        IsAdmin = isAdmin,
                        AuthToken = dto.AuthToken,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedTemplatePrompts = x.UpdatedTemplatePrompts ?? null,
                        UpdatedTemplateTitle = x.UpdatedTemplateTitle ?? null,
                    });
                });

                var result = await _userTemplateRepository.AddRangeAsync(userTemplates);
                return new Response<bool> { Success = result != null, Message = result != null ? StatusAndMessagesKeys.CreateSuccess : StatusAndMessagesKeys.CreateFailed };
            }
            
            return new Response<bool>
            {
                Message = StatusAndMessagesKeys.CreateFailed + ", SectionsIds must be provided",
                Success = false
            };
            

        }

        public async Task<PaginatedResponse<UserTemplateSpecificationDto>> GetAllUserTemplateAsync(UserTemplateFilterDto filter)
        {
            var (result, totalCount) = await _userTemplateRepository.GetUserTemplatesAsync(filter);

            var templates = result
                .GroupBy(x => x.TemplateId)
                .Select(group => new UserTemplateSpecificationDto
                {
                    Id = group.Key ?? 0,
                    Name = group.First().Template?.Name,
                    AuthToken = group.First().AuthToken,
                    Description = group.First().Template?.Description,
                    Language = group.First().Template?.Language,
                    TemplateCategoryId = group.First().Template?.TemplateCategoryId,
                    CreatedDate = group.First().Template?.CreatedDate,
                    IsCreatedByAdmin = group.First().Template?.IsCreatedByAdmin,
                    CreatedBy = group.First().Template?.CreatedBy,
                    ImagePath = group.First().Template?.ImagePath,
                    TemplateCategory = group.First().Template?.TemplateCategory != null
                        ? new TemplateCategoryDto
                        {
                            Id = group.First().Template.TemplateCategory.Id,
                            Name = group.First().Template.TemplateCategory.Name
                        }
                        : null,
                    templateSubSections = group
                        .Where(t => t.TemplateSubSectionId.HasValue)
                        .Select(t => new TemplateSubSectionDto
                        {
                            Id = t?.TemplateSubSectionId,
                            Prompt = t?.TemplateSubSection?.Prompt,
                            Title = t?.TemplateSubSection?.Title,
                            TemplateSectionId = t?.TemplateSubSection?.TemplateSectionId,
                            TemplateSectionName = t?.TemplateSubSection?.TemplateSection?.Title,
                            SubSectionOrder = t?.SubSectionOrder,
                            UserTemplateId = t?.Id,
                            UpdatedTemplatePrompts = t?.UpdatedTemplatePrompts,
                            UpdatedTemplateTitle = t?.UpdatedTemplateTitle
                        })
                        .OrderBy(x => x.SubSectionOrder)
                        .ToList()
                })
                .ToList();

            if (filter.PageNumber.HasValue && filter.PageSize.HasValue)
            {
                int skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;


                templates = templates.Skip(skip).Take(filter.PageSize.Value).ToList();
            }

            int pageNumber = filter.PageNumber ?? 1;
            int pageSize = filter.PageSize ?? 10;

            var paginatedResult = new PaginatedResult<UserTemplateSpecificationDto>
            {
                Items = templates,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return new PaginatedResponse<UserTemplateSpecificationDto>
            {
                Success = true,
                Message = StatusAndMessagesKeys.GetAllSuccess,
                Data = paginatedResult
            };
        }


        public async Task<ResponseList<UserTemplateDto>> GetAllByCategoryAsync(int categoryId)
        {
            var result = await _templateRepository.GetAllUserTemplateByCategoryAsync(categoryId);
            var dtos = result.ConvertAll(u => new UserTemplateDto(u));
            return new ResponseList<UserTemplateDto> { Data = dtos, Success = true };
        }

        public async Task<Response<bool>> UpdateUserTemplateAsync(UpdateUserTemplateDto dto)
        {
            var userTemplate = new UserTemplate
            {
                Id = dto.Id,
                TemplateId = dto.TemplateId,
                //TemplateSectionId = dto.TemplateSectionId,
                TemplateSubSectionId = dto.TemplateSubSectionId,
                ModifiedDate = DateTime.UtcNow
            };
            var result = await _templateRepository.UpdateUserTemplateAsync(userTemplate);
            return new Response<bool> { Data = result, Success = result, Message = result ? "Updated successfully" : "Update failed" };
        }

        public async Task<ResponseList<TemplateSectionDto>> GetAllTemplateSubSectionsAsync()
        {
            ResponseList<TemplateSectionDto> response = new ResponseList<TemplateSectionDto>();
            try
            {
                var templateSections = await _templateRepository.GetAllTemplateSubSectionsAsync();

                response.Data = templateSections.Select(section => new TemplateSectionDto
                {
                    Title = section.Title,
                    Priority = section.Priority,
                    TemplateSubSections = section.TemplateSubSections?.Select(sub => new TemplateSubSectionDto
                    {
                        Id = sub.Id,
                        TemplateSectionId = sub.TemplateSectionId,
                        Prompt = sub.Prompt,
                        Title = sub.Title,
                    }).ToList()
                }).ToList();

                response.Message = templateSections.Any() ? StatusAndMessagesKeys.GetAllSuccess : StatusAndMessagesKeys.GetAllNoData;
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching TemplateSubSections.");
                response.Message = "An error occurred while fetching TemplateSubSections.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<bool>> CreateUserTemplateFrequencyAsync(CreateUserTemplateFrequencyDto dto)
        {
            Response<bool> response = new Response<bool>();

            try
            {

                var result = await _templateRepository.CreateUserTemplateFrequencyAsync(new UserTemplateFrequency
                {
                    TemplateId = dto.TemplateId,
                    CreatedBy = dto.CreatedBy,
                    AuthToken = dto.AuthToken,
                    CreatedDate = DateTime.UtcNow
                });

                response.Data = result ? true : false;
                response.Message = result ? StatusAndMessagesKeys.CreateSuccess : StatusAndMessagesKeys.CreateFailed;
                response.Success = result ? true : false;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating UserTemplateFrequency.");
                response.Message = "An error occurred while creating UserTemplateFrequency.";
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<bool>> DeleteUserTemplatesAsync(DeleteUserTemplateDto templateDto)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                if (string.IsNullOrEmpty(templateDto.AuthToken))
                {
                    response.Message = StatusAndMessagesKeys.NoDataFound + " AuthToken " + templateDto.AuthToken;
                    response.Success = false;
                    return response;
                }
                
                var isDeleted = await _templateRepository.DeleteUserTemplatesAsync(templateDto.TemplateId, templateDto.AuthToken);

                if (!isDeleted)
                {
                    response.Message = "No user templates found for deletion.";
                    response.Success = false;
                    return response;
                }

                response.Data = true;
                response.Message = StatusAndMessagesKeys.DeleteSuccess;
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting UserTemplates.");
                response.Message = "An error occurred while deleting UserTemplates.";
                response.Success = false;
                return response;
            }
        }


    }
}
