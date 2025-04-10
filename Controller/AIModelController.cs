using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Text;

namespace GenAiPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIModelController : Controller
    {
        private readonly IAIModelService _aiModelService;

        public AIModelController(IAIModelService aiModelService)
        {
            this._aiModelService = aiModelService;
        }


        /// <summary>
        /// Adds a new ModelVersionDetail.
        /// </summary>
        [HttpPost("AddModelVersionDetail")]
        public async Task<Response<bool>> AddModelVersionDetail(AddModelVersionDetailDto request)
        {
            var result = await _aiModelService.AddModelVersionDetailAsync(request);
            return result;
        }

        /// <summary>
        /// Retrieves all ModelVersionDetails.
        /// </summary>
        [HttpPost("GetModelVersionDetails")]
        public async Task<ResponseList<ModelVersionDetail>> GetModelVersionDetails()
        {
            var result = await _aiModelService.GetModelVersionDetailsAsync();
            return result;
        }


        [HttpPost("QueryChatStream")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task QueryChatStream(ChatHistoryRequest request)
        {
            Response.ContentType = "text/event-stream";

            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AccessToken = accessToken;
            }

            await foreach (var line in _aiModelService.ModelResponseStream(request))
            {
                var bytes = Encoding.UTF8.GetBytes($"data: {line}\n\n");
                await Response.Body.WriteAsync(bytes);
                await Response.Body.FlushAsync();
            }
        }
    }
}
