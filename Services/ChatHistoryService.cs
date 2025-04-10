using AutoMapper;
using GenAiPoc.Contracts.Models;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Enums;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Application.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly ICodeBuddyRepository _workspaceRepository;
        public ChatHistoryService(ICodeBuddyRepository workspaceRepository, IBlobStorageService blobStorageService, IMapper mapper)
        {
            _workspaceRepository = workspaceRepository;
        }
        public async Task<GetChatHistoryResponse> GetChatHistoryByAuthToken(string AuthToken,int? historyChatType)
        {
            try
            {
                var chats = await _workspaceRepository.GetChatHistoryByAuthToken(AuthToken, historyChatType);
                if (chats.Count > 0)
                {
                    return new GetChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, chats);
                }
                else
                {
                    return new GetChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, chats);
                }
            }
            catch (Exception ex)
            {
                return new GetChatHistoryResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        
        public async Task<GetChatHistoryResponse> GetChatHistoryByWorkspaceId(int Id)
        {
            try
            {
                var chats = await _workspaceRepository.GetChatHistoryByWorkspaceId(Id);
                if (chats.Count > 0)
                {
                    return new GetChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, chats);
                }
                else
                {
                    return new GetChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, chats);
                }
            }
            catch (Exception ex)
            {
                return new GetChatHistoryResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetChatPromptsResponse> GetChatPrmoptsAsync()
        {
            try
            {
                var prmopts = await _workspaceRepository.GetChatPrompts();
                if (prmopts.Count > 0)
                {
                    return new GetChatPromptsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, prmopts);
                }
                else
                {
                    return new GetChatPromptsResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, prmopts);
                }
            }
            catch (Exception ex)
            {
                return new GetChatPromptsResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        public async Task<GetSessionChatHistoryResponse> GetSessionChatHistoryByAuthToken(string AuthToken, int? historyChatType)
        {
            try
            {
                var chats = await _workspaceRepository.GetSessionChatHistoryByAuthToken(AuthToken, historyChatType);
                if (chats.Count > 0)
                {
                    return new GetSessionChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.SuccessMessage, chats);
                }
                else
                {
                    return new GetSessionChatHistoryResponse(StatusAndMessagesKeys.SuccessStatus, StatusAndMessagesKeys.NoDataFound, chats);
                }
            }
            catch (Exception ex)
            {
                return new GetSessionChatHistoryResponse(StatusAndMessagesKeys.ErrorStatus, StatusAndMessagesKeys.SomethingWentWrong, null);
            }
        }
        
    }
}
