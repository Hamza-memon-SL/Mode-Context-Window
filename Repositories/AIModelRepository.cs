using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Infrastructure.Repository
{
    public class AIModelRepository : IAIModelRepository
    {
        private readonly DbContextGenAiPOC _dbContextGenAiPOC;

        public AIModelRepository(DbContextGenAiPOC dbContextGenAiPOC)
        {
            this._dbContextGenAiPOC = dbContextGenAiPOC;
        }

        public async Task<bool> AddModelVersionDetail(ModelVersionDetail request)
        {
            try
            {
                await _dbContextGenAiPOC.ModelVersionDetails.AddAsync(request);
                await _dbContextGenAiPOC.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ModelVersionDetail>> GetModelVersionDetails()
        {
            try
            {
                var result = await _dbContextGenAiPOC.ModelVersionDetails
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return new List<ModelVersionDetail>();
            }
        }
    }
}
