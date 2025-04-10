using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Response;
using GenAiPoc.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IChatHistoryService
    {
        Task<GetChatHistoryResponse> GetChatHistoryByWorkspaceId(int Id);
        Task<GetChatHistoryResponse> GetChatHistoryByAuthToken(string AuthToken,int? historyChatType);
        Task<GetChatPromptsResponse> GetChatPrmoptsAsync();
        Task<GetSessionChatHistoryResponse> GetSessionChatHistoryByAuthToken(string AuthToken, int? historyChatType);
    }
}
