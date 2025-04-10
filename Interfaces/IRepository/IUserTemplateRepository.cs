using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using GenAiPoc.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IRepository
{
    public interface IUserTemplateRepository : IRepository<UserTemplate>
    {
        Task<(List<UserTemplate> Templates, int TotalCount)> GetUserTemplatesAsync(UserTemplateFilterDto filter);

    }
}
