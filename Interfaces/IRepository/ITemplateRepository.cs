using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface ITemplateRepository
    {
        Task CreateTemplateCategoryAsync(TemplateCategory templateCategory);
        Task<List<TemplateCategory>> GetAllTemplateCategoryAsync();
        Task<bool> CreateTemplateAsync(Template template);
        Task<List<Template>> GetAllTemplatesAsync(int page, int pageSize, string? searchTerm);
        Task<Template?> GetTemplateByIdAsync(int id);
        Task<List<Template>> GetTemplatesByCategoryIdAsync(int categoryId);
        Task<bool> UpdateTemplateAsync(UpdateTemplateDto updateDto);

        Task<bool> CreateTemplateSectionAsync(TemplateSection entity);
        Task<List<TemplateSection>> GetAllTemplateSectionByTemplateIdAsync(int templateId);
        Task<TemplateSection> GetTemplateSectionByIdAsync(int id);
        Task<bool> UpdateTemplateSectionAsync(TemplateSection entity);
        Task<bool> DeleteTemplateSectionAsync(int id);


        Task<bool> CreateUserTemplateAsync(UserTemplate userTemplate);
        Task<List<UserTemplate>> GetAllUserTemplateByAdminAsync(UserTemplateFilterDto filter);
        Task<List<UserTemplate>> GetAllUserTemplateByCategoryAsync(int categoryId);
        Task<bool> UpdateUserTemplateAsync(UserTemplate userTemplate);

        Task<List<TemplateSection>> GetAllTemplateSubSectionsAsync();

        Task<bool> CreateUserTemplateFrequencyAsync(UserTemplateFrequency entity);
        Task<bool> DeleteUserTemplatesAsync(int id, string authToken);


    }
}
