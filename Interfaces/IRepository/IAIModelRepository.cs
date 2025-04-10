using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
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
    public interface IAIModelRepository
    {
        Task<bool> AddModelVersionDetail(ModelVersionDetail request);
        Task<List<ModelVersionDetail>> GetModelVersionDetails();
    }
}
