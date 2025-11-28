using Worksy.Web.Core;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Services.Abstractions;

public interface IConversationService
{
    Task<Response<Conversation?>> GetConversationAsync(Guid id);
    Task<Response<Conversation?>> FindBetweenUsersAsync(Guid userId, Guid user2Id, Guid serviceId);
    Task<Response<Conversation>> CreateConversationAsync(Guid customerId, Guid serviceId);
    Task<Response<List<Conversation>>> GetConversationsByProviderAsync(Guid providerId);
    Task<Response<List<Conversation>>> GetConversationsAsync(Guid userId);
}