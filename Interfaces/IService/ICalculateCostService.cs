using GenAiPoc.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface ICalculateCostService
    {
        public Task<List<CodeBuddyFileDetails>> CalculateTokensAsync(int modelID, List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList);
        public Task<List<CodeBuddyFileDetails>> CalculateTokensAsync(string platform, List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList);
        public Task<List<CodeBuddyFileDetails>> CalculateClaudeTokens(List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList); //input=list of file contents in a folder, output=FileDetails.tokens
        public Task<List<string>> ReadAllFilesContent(List<string> filePaths);
        public Task<List<CodeBuddyFileDetails>> CalculateOpenAITokens(List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList);
        public Task<decimal> CalculateAssumedCost(decimal inputTokenPrice, List<CodeBuddyFileDetails> fileDetailsList);

        public Task<decimal> CalculateActualCost(decimal FilesCost, decimal ProjectsCost);
        public Task<decimal> CalculateProjectsActualCost(decimal inputTokenPrice, decimal outputTokenPrice, CodeBuddyProjects project);
        public Task<decimal> CalculateFilesActualCost(decimal inputTokenPrice, decimal outputTokenPrice, List<CodeBuddyFileDetails> fileDetailsList);

    }
}
