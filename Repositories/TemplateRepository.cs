using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using GenAiPoc.Core.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Infrastructure.Repository
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly DbContextGenAiPOC _dbContext;

        public TemplateRepository(DbContextGenAiPOC dbContextGenAiPOC)
        {
            this._dbContext = dbContextGenAiPOC;
        }

        public async Task CreateTemplateCategoryAsync(TemplateCategory templateCategory)
        {
            await _dbContext.TemplateCategories.AddAsync(templateCategory);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<List<TemplateCategory>> GetAllTemplateCategoryAsync()
        {
            return await _dbContext.TemplateCategories.ToListAsync();
        }


        public async Task<bool> CreateTemplateAsync(Template template)
        {
            _dbContext.Templates.Add(template);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Template>> GetAllTemplatesAsync(int page, int pageSize, string? searchTerm)
        {
            var query = _dbContext.Templates.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Name.Contains(searchTerm) || t.Description.Contains(searchTerm));
            }

            return await query.OrderByDescending(t => t.IsActive) // what should be the frequently used logic? 
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

        public async Task<Template?> GetTemplateByIdAsync(int id)
        {
            return await _dbContext.Templates
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Template>> GetTemplatesByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.Templates
                .Where(t => t.TemplateCategoryId == categoryId)
                .ToListAsync();
        }
        public async Task<bool> UpdateTemplateAsync(UpdateTemplateDto updateDto)
        {
            try
            {
                var template = await _dbContext.Templates
                    .FirstOrDefaultAsync(t => t.Id == updateDto.TemplateId && (t.AuthToken == updateDto.AuthToken || t.IsCreatedByAdmin == true));

                if (template == null)
                    return false;

                // Update template properties
                template.Name = updateDto.Name ?? template.Name;
                template.Description = updateDto.Description ?? template.Description;
                template.Language = updateDto.Language ?? template.Language;
                template.TemplateCategoryId = updateDto.TemplateCategoryId ?? template.TemplateCategoryId;
                template.ImagePath = updateDto.ImagePath;

                var userTemplateIds = updateDto?.UserTemplatesIds?
                    .Select(ut => ut.UserTemplateId).ToList();

                // Fetch existing UserTemplates related to the given template
                var existingUserTemplates = await _dbContext.UserTemplates
                    .Where(ut => ut.TemplateId == template.Id && ut.AuthToken == updateDto.AuthToken)
                    .ToListAsync();

                // Step-1 Delete extra records** (Remove records that are not in the update list)
                var userTemplatesToDelete = existingUserTemplates
                    .Where(ut => !userTemplateIds.Contains(ut.Id))
                    .ToList();

                if (userTemplatesToDelete.Any())
                {
                    _dbContext.UserTemplates.RemoveRange(userTemplatesToDelete);
                }

                // Step-2 Update existing records**

                var userTemplates = await _dbContext.UserTemplates
                    .Where(ut => userTemplateIds.Contains(ut.Id))
                    .ToListAsync();

                userTemplates.ForEach(ut =>
                {
                    var updateData = updateDto?.UserTemplatesIds?.FirstOrDefault(u => u.UserTemplateId == ut.Id);
                    if (updateData != null)
                    {
                        ut.SubSectionOrder = updateData.Order;
                        ut.TemplateSubSectionId = updateData.SubSectionId;
                        ut.UpdatedTemplatePrompts = updateData.UpdatedTemplatePrompts;
                        ut.UpdatedTemplateTitle = updateData.UpdatedTemplateTitle;

                    }
                });

                //Step-3 add existing records

                var templateToBeAdded = updateDto?.UserTemplatesIds?
                    .Where(x => x.UserTemplateId == 0).ToList();

                if(templateToBeAdded != null && templateToBeAdded.Any())
                {
                    List<UserTemplate> userTemplatesToBeAdded = new List<UserTemplate>();
                    foreach (var userTemp in templateToBeAdded)
                    {
                        UserTemplate userTemplate = new UserTemplate
                        {
                            AuthToken = updateDto.AuthToken,
                            CreatedBy = updateDto.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            //IsAdmin = updateDto.IsCreatedByAdmin,
                            IsAdmin = template.IsCreatedByAdmin,
                            SubSectionOrder = userTemp.Order,
                            TemplateSubSectionId = userTemp.SubSectionId,
                            TemplateId = updateDto.TemplateId,
                            IsActive = true,
                            UpdatedTemplatePrompts = userTemp.UpdatedTemplatePrompts ?? null,
                            UpdatedTemplateTitle = userTemp.UpdatedTemplateTitle ?? null
                        };

                        userTemplatesToBeAdded.Add(userTemplate);

                    }

                    await _dbContext.AddRangeAsync(userTemplatesToBeAdded);
                }

                await _dbContext.SaveChangesAsync();              
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating template: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateTemplateSectionAsync(TemplateSection entity)
        {
            await _dbContext.TemplateSections.AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<TemplateSection>> GetAllTemplateSectionByTemplateIdAsync(int templateId)
        {
            return await _dbContext.TemplateSections.Where(ts =>  ts.IsActive == true).ToListAsync();
        }

        public async Task<TemplateSection> GetTemplateSectionByIdAsync(int id)
        {
            return await _dbContext.TemplateSections.FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<bool> UpdateTemplateSectionAsync(TemplateSection entity)
        {
            _dbContext.TemplateSections.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTemplateSectionAsync(int id)
        {
            var entity = await _dbContext.TemplateSections.FirstOrDefaultAsync(ts => ts.Id == id);
            if (entity == null) return false;

            _dbContext.TemplateSections.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> CreateUserTemplateAsync(UserTemplate userTemplate)
        {
            await _dbContext.UserTemplates.AddAsync(userTemplate);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<UserTemplate>> GetAllUserTemplateByAdminAsync(UserTemplateFilterDto filter)
        {
            return await _dbContext.UserTemplates
                .Where(ut => ut.CreatedBy == filter.AuthToken || filter.TemplateId == ut.TemplateId)
                .ToListAsync();
        }

        public async Task<List<UserTemplate>> GetAllUserTemplateByCategoryAsync(int categoryId)
        {
            return await _dbContext
                .UserTemplates
                .Include(x=>x.Template)
                .Where(ut => ut.Template.TemplateCategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserTemplateAsync(UserTemplate userTemplate)
        {
            _dbContext.UserTemplates.Update(userTemplate);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<TemplateSection>> GetAllTemplateSubSectionsAsync()
        {
            try
            {
                return await _dbContext
                    .TemplateSections
                    .Include(x=>x.TemplateSubSections)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching TemplateSubSections: {ex.Message}");
                return new List<TemplateSection>();
            }
        }

        public async Task<bool> CreateUserTemplateFrequencyAsync(UserTemplateFrequency entity)
        {
            try
            {
                await _dbContext.UserTemplateFrequencies.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating UserTemplateFrequency: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeleteUserTemplatesAsync(int id, string authToken)
        {
            try
            {
                var template = await _dbContext.Templates
                    .FirstOrDefaultAsync(x => x.Id == id && x.AuthToken==authToken);

                if (template == null)
                    return false;

                var userTemplatesToDelete = await _dbContext.UserTemplates
                    .Where(ut => ut.TemplateId == template.Id )
                    .ToListAsync();

                if (userTemplatesToDelete.Any())
                {                    
                    _dbContext.UserTemplates.RemoveRange(userTemplatesToDelete);
                }

                var userFrequencies = await _dbContext.UserTemplateFrequencies
                    .Where(ut => ut.TemplateId == template.Id)
                    .ToListAsync();

                if (userFrequencies.Any()) { 
                    
                    _dbContext.UserTemplateFrequencies.RemoveRange(userFrequencies);
                }

                _dbContext.Templates.Remove(template);
                await _dbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user templates: {ex.Message}");
                return false;
            }
        }

    }
}
