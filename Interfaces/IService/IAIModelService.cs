using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Request;
using GenAiPoc.Core.Response;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IAIModelService
    {
        Task<Response<AIModelDTO>> GetAllAIModelsAsync(GetAiModelsRequests request);
        Task<Response<AIModelDTO>> GetModelsForAllUserGroupsRequests(GetModelsForAllUserGroupsRequests request);

        Task<Response<AIModelKeyDTO>> GetAIModelApiKeyAsync(GetAiModelApiKeyRequests request);
        Task<Response<GetUserGroupModelsResponse>> GetAllDynamicModelsAsync(GetDynamicModelRequests request);
        Task<Response<GetAllUserGroupModelsResponse>> GetAllUserGroupDynamicModelsAsync(GetModelsForAllGroupsRequest request);
        Task<Response<GetAllUserGroupModelsResponse>> GetAllUserGroupDynamicModelsV2Async(GetModelsForAllGroupsByUsernameRequest request);
        Task<Response<bool>> AddModelVersionDetailAsync(AddModelVersionDetailDto request);
        Task<ResponseList<ModelVersionDetail>> GetModelVersionDetailsAsync();
        IAsyncEnumerable<string> ModelResponseStream(ChatHistoryRequest request);

    }
}
