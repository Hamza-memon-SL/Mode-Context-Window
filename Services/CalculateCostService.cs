using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using GenAiPoc.Contracts.Models;
using LLMSharp.Anthropic.Tokenizer;
using Microsoft.Extensions.Logging;
using SharpToken;
using GenAiPoc.Core.Interfaces.IService;

namespace GenAiPoc.Application.Services
{
    public class CalculateCostService: ICalculateCostService
    {
        private readonly ILogger<CalculateCostService> _logger;
        public CalculateCostService(ILogger<CalculateCostService> logger) {
        
            _logger = logger;
        }
        public async Task<List<CodeBuddyFileDetails>> CalculateTokensAsync(int modelID, List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList)
        {

            if (contentList.Count != fileDetailsList.Count)
            {
                throw new ArgumentException("contentList and fileDetailsList must have the same number of elements.");
            }

           
            return modelID switch
            {
                1 => await CalculateOpenAITokens(contentList, fileDetailsList),
                2 => await CalculateClaudeTokens(contentList, fileDetailsList),
                _ => fileDetailsList  // Return original list if modelID is unrecognized
            };
        }

        public async Task<List<CodeBuddyFileDetails>> CalculateTokensAsync(string platform, List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList)
        {

            if (contentList.Count != fileDetailsList.Count)
            {
                throw new ArgumentException("contentList and fileDetailsList must have the same number of elements.");
            }


            return platform.ToLower() switch
            {
                "azure" => await CalculateOpenAITokens(contentList, fileDetailsList),
                "aws" => await CalculateClaudeTokens(contentList, fileDetailsList),
                _ => fileDetailsList  // Return original list if modelID is unrecognized
            };
        }

        public Task<decimal> CalculateAssumedCost(decimal inputTokenPrice,List<CodeBuddyFileDetails> fileDetailsList)
        {
            var projectCost = 0.0m;
            for (int i = 0; i < fileDetailsList.Count; i++)
            {
                if (fileDetailsList[i] == null )
                {
                    throw new Exception($"Tokens property is null for file at index {i}");
                }
                decimal AssumedInputTokens = fileDetailsList[i].AssumedInputTokens ?? 0.0m;
               
                decimal singlefileCost = ((decimal)((AssumedInputTokens / 1000000.0m) * inputTokenPrice));
                projectCost = singlefileCost+projectCost;
            }
            return Task.FromResult(projectCost);

        }

        public Task<decimal> CalculateFilesActualCost(decimal inputTokenPrice, decimal outputTokenPrice, List<CodeBuddyFileDetails> fileDetailsList) //on based of files
        {
            var projectCost = 0.0m;
            for (int i = 0; i < fileDetailsList.Count; i++)
            {
                if (fileDetailsList[i] == null)
                {
                    throw new Exception($"Tokens property is null for file at index {i}");
                }
                decimal actualInputTokens = fileDetailsList[i].ActualInputTokens ?? 0.0m;
                decimal actualOutputTokens = fileDetailsList[i].ActualOutputTokens ?? 0.0m;
                decimal singleFileCost = ((actualInputTokens / 1000000.0m) * inputTokenPrice) + ((actualOutputTokens / 1000000.0m) * outputTokenPrice);
                projectCost = singleFileCost + projectCost;
            }
            return Task.FromResult(projectCost);

        }
        public Task<decimal> CalculateProjectsActualCost(decimal inputTokenPrice, decimal outputTokenPrice, CodeBuddyProjects project) //on based of projects last call to llm
        {
            var projectCost = 0.0m;
            if (project == null)
                {
                    throw new Exception($"Tokens property is null for project");
                }
                decimal actualInputTokens = project.ActualInputTokens ?? 0;
                decimal actualOutputTokens = project.ActualOutputTokens ?? 0;
                decimal singleFileCost = ((actualInputTokens / 1000000.0m) * inputTokenPrice) + ((actualOutputTokens / 1000000.0m) * outputTokenPrice);
                projectCost = singleFileCost + projectCost;
            
            return Task.FromResult(projectCost);

        }

        public Task<decimal> CalculateActualCost(decimal FilesCost, decimal ProjectsCost) //on based of files
        {

            decimal actualCost = FilesCost + ProjectsCost;
                
                
            
            return Task.FromResult(actualCost);

        }
      
        public Task<List<CodeBuddyFileDetails>> CalculateClaudeTokens(List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList)
        {
            var tokenizer = new ClaudeTokenizer();
            for (int i = 0; i < contentList.Count; i++)
            {
                var content = contentList[i];
                var tokenCount = tokenizer.CountTokens(content);

                fileDetailsList[i].AssumedInputTokens = tokenCount;
            }

            return Task.FromResult(fileDetailsList);

        }
        public Task<List<CodeBuddyFileDetails>> CalculateOpenAITokens(List<string> contentList, List<CodeBuddyFileDetails> fileDetailsList)
        {
            var encoding = GptEncoding.GetEncoding("o200k_base");
            for (int i = 0; i < contentList.Count; i++)
            {
                var content = contentList[i];
                var tokenCount = encoding.CountTokens(content);

                fileDetailsList[i].AssumedInputTokens = tokenCount;
            }

            return Task.FromResult(fileDetailsList);

        }

        public async Task<List<string>> ReadAllFilesContent(List<string> filePaths)
        {
            _logger.LogInformation("Reading File Content method start");
            try
            {
                return filePaths
                    .Select(File.ReadAllText) 
                    .ToList(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading files: {ex.Message}");
                return new List<string>();
            }
        }

        
    }
}
