using Worksy.Web.Core;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Services.Abstractions;

public interface IMessageService
{
    Task<Response<List<Message>>> GetMessagesAsync(Guid conversationId);
    Task<Response<Message>> AddMessageAsync(Guid conversationId, Guid senderId, string content);
}