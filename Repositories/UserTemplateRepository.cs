using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Response;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Infrastructure.Repository
{
    public class UserTemplateRepository : Repository<UserTemplate>, IUserTemplateRepository
    {
        private readonly DbContextGenAiPOC _context;

        public UserTemplateRepository(DbContextGenAiPOC context) : base(context)
        {
            this._context = context;
        }

        //public async Task<PaginatedResponse<UserTemplate>> GetUserTemplatesAsync(UserTemplateFilterDto filter)
        //{
        //    IQueryable<UserTemplate> query = _context.UserTemplates
        //        .Include(ut => ut.Template)
        //        .ThenInclude(tc => tc.TemplateCategory)
        //        .Include(x => x.UserTemplateFrequencies)
        //        .Include(x => x.TemplateSubSection)
        //        .ThenInclude(x=>x.TemplateSection);
                

        //    // Apply filtering criteria
        //    query = query.Where(ut =>
        //        !string.IsNullOrEmpty(filter.AuthToken) &&
        //        (ut.CreatedBy == filter.AuthToken || ut.IsAdmin == true) &&
        //        (!filter.TemplateId.HasValue || ut.TemplateId == filter.TemplateId) &&
        //        (!filter.CategoryId.HasValue || ut.Template.TemplateCategoryId == filter.CategoryId) &&
        //        (string.IsNullOrEmpty(filter.Search) || ut.Template.Name.Contains(filter.Search) || ut.Template.Description.Contains(filter.Search))
        //    );

        //    int totalCount = await query.CountAsync();

        //    // Apply sorting
        //    switch (filter.SortingCriteria)
        //    {
        //        case "frequently_used":
        //            query = query.OrderByDescending(ut => ut.UserTemplateFrequencies.Count());
        //            break;
        //        case "recently_used":
        //            query = query.OrderByDescending(ut => ut.UserTemplateFrequencies.Max(t => t.CreatedDate));
        //            break;
        //        case "recently_added":
        //            query = query.OrderByDescending(ut => ut.CreatedDate);
        //            break;
        //        default:
        //            query = query.OrderByDescending(ut => ut.CreatedDate);
        //            break;
        //    }
        //    int pageNumber = filter.PageNumber ?? 1;
        //    int pageSize = filter.PageSize ?? 10;
        //    // Apply pagination
        //    if (filter.PageNumber.HasValue && filter.PageSize.HasValue)
        //    {
        //        int skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;
        //        query = query.Skip(skip).Take(filter.PageSize.Value);
        //    }

        //    var paginatedResult = new PaginatedResult<UserTemplate>
        //    {
        //        Items = await query.ToListAsync(),
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize
        //    };

        //    return new PaginatedResponse<UserTemplate>
        //    {
        //        Success = true,
        //        Message = "User templates fetched successfully.",
        //        Data = paginatedResult
        //    };

        //  //  return await query.ToListAsync();
        //}

        public async Task<(List<UserTemplate> Templates, int TotalCount)> GetUserTemplatesAsync(UserTemplateFilterDto filter)
        {
            IQueryable<UserTemplate> query = _context.UserTemplates
                .Include(ut => ut.Template)
                    .ThenInclude(tc => tc.TemplateCategory)

                    .Include(ut => ut.Template)
                    .ThenInclude(ut => ut.UserTemplateFrequencies)
                //.Include(x => x.UserTemplateFrequencies)
                .Include(x => x.TemplateSubSection)
                    .ThenInclude(x => x.TemplateSection);

            // Apply filtering criteria
            query = query.Where(ut =>
                !string.IsNullOrEmpty(filter.AuthToken) &&
                (ut.AuthToken == filter.AuthToken || ut.IsAdmin == true) &&
                (!filter.TemplateId.HasValue || ut.TemplateId == filter.TemplateId) &&
                (!filter.CategoryId.HasValue || ut.Template.TemplateCategoryId == filter.CategoryId) &&
                (string.IsNullOrEmpty(filter.Search) || ut.Template.Name.Contains(filter.Search) || ut.Template.Description.Contains(filter.Search))
            );

            // Get total count before pagination
            int totalCount = await _context.UserTemplates
                .Where(x=>x.AuthToken == filter.AuthToken || x.IsAdmin == true)
                .GroupBy(x=>x.TemplateId)
                .CountAsync();

            // Apply sorting
            switch (filter.SortingCriteria)
            {
                case "frequently_used":
                    query = query.OrderByDescending(ut => ut.Template.UserTemplateFrequencies.Count())
                        .ThenByDescending(x=>x.CreatedDate);
                    break;
                case "recently_used":
                    query = query.OrderByDescending(ut => ut.Template.UserTemplateFrequencies.Max(t => t.CreatedDate))
                        .ThenByDescending(x => x.CreatedDate);
                    break;
                case "recently_added":
                    query = query.OrderByDescending(ut => ut.CreatedDate);
                    break;
                default:
                    query = query.OrderByDescending(ut => ut.CreatedDate);
                    break;
            }

            var result = await query.ToListAsync();

            // Apply pagination
            //if (filter.PageNumber.HasValue && filter.PageSize.HasValue)
            //{
            //    int skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;
            //    query = query.Skip(skip).Take(filter.PageSize.Value);
            //}
            //var result = await query.ToListAsync();

            List<UserTemplate> templates = result;

            return (templates, totalCount);
        }

    }
}
