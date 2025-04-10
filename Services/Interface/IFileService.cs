using Function.HttpTriggerChat.Domain.DTO;
using Function.HttpTriggerChat.Domain.Request;
using Function.HttpTriggerChat.Domain.Response;
using GenAiPoc.Core.Entities;
using Package.CustomAiModel.DTO;
using Package.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.HttpTriggerChat.Service.Interface
{
    public interface IFileService
    {
        Task<List<ChatHistory>> Query(ChatHistoryDTO question);
        Task<List<ChatHistory>> QueryV2(ChatHistoryRequest question);
        IAsyncEnumerable<string> QueryV2Stream(ChatHistoryRequest question);
        Task<string> GetUserStory(UserStoryRequest question);
        Task<string> GetUserInterface(UserInterfaceRequest request);
        Task<string> GetUserInterfaceV2(UserInterfaceRequest request);
        Task<string> AnalyzeImage(UserInterfaceRequest request);
        Task<GetGlobalAnalytics> GetGlobalAnalyticsAsync();
        Task<GetUserSpecificAnalytics> GetUserSpecificAnalyticsAsync(string authToken);
        Task<MainFrameDestinationFiles> GetMainFrameDestinationFileDetailById(int fileId);
        Task<string> ReadFileContent(string filePaths);
        Task UpdateMainFrameDestinationFileDetailsAsync(string summary, MainFrameDestinationFiles fileDetails);
        Task<string> MainFrameSimpleCodeSummarize(ChatHistoryRequest question);
        Task<MainFrameConfigurations> GetMainFrameConfigurations();
        Task<Configurations> GetConfigurations();
        Task<string> GetJsonFromResponse(string aiResponse);
        Task<List<ArtifactCategory>> GetArtifactCategoryAsync();
        Task<string> CreateArtifactGeneratorUserStoryAsync(CreateArtifactGeneratorUserStoryDTO dto);
    }
}
