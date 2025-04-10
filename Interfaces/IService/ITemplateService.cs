using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface ITemplateService
    {
        Task<Response<bool>> CreateTemplateCategoryAsync(CreateTemplateCategoryDto dto);
        Task<ResponseList<TemplateCategoryDto>> GetAllTemplateCategoryAsync();

        Task<Response<bool>> CreateTemplateAsync(CreateTemplateDto templateDto);
        Task<ResponseList<TemplateDto>> GetAllTemplatesAsync(int page, int pageSize, string? searchTerm);
        Task<Response<TemplateDto>> GetTemplateByIdAsync(int id);
        Task<ResponseList<TemplateDto>> GetTemplatesByCategoryIdAsync(int categoryId);
        Task<Core.Response.Response<bool>> UpdateTemplateAsync(UpdateTemplateDto templateDto);
        Task<Response<bool>> CreateTemplateSectionAsync(TemplateSectionDTO templateSectionDto);
        Task<ResponseList<TemplateSectionDTO>> GetAllTemplateSectionByTemplateIdAsync(int templateId);
        Task<Response<bool>> UpdateTemplateSectionAsync(TemplateSectionDTO templateSectionDto);
        Task<Response<bool>> DeleteTemplateSectionAsync(int id);

        Task<Response<bool>> CreateUserTemplateAsync(CreateUserTemplateDto dto);
        //Task<ResponseList<UserTemplateSpecificationDto>> GetAllUserTemplateAsync(UserTemplateFilterDto filter);
        Task<PaginatedResponse<UserTemplateSpecificationDto>> GetAllUserTemplateAsync(UserTemplateFilterDto filter);
        Task<ResponseList<UserTemplateDto>> GetAllByCategoryAsync(int categoryId);
        Task<Response<bool>> UpdateUserTemplateAsync(UpdateUserTemplateDto dto);
        Task<ResponseList<TemplateSectionDto>> GetAllTemplateSubSectionsAsync();

        Task<Response<bool>> CreateUserTemplateFrequencyAsync(CreateUserTemplateFrequencyDto dto);

        Task<Response<bool>> DeleteUserTemplatesAsync(DeleteUserTemplateDto deleteUser);

    }
}
