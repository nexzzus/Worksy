using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class ConversationService : IConversationService
{
    private readonly DataContext _context;

    public ConversationService(DataContext context)
    {
        _context = context;
    }

    public async Task<Response<Conversation?>> GetConversationAsync(Guid id)
    {
        Conversation? conversation = await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation == null)
        {
            return Response<Conversation?>.Failure("Conversación no encontrada.");
        }

        return Response<Conversation?>.Success(conversation, "Conversación obtenida con éxito.");
    }

    public async Task<Response<Conversation?>> FindBetweenUsersAsync(Guid userId, Guid user2Id, Guid serviceId)
    {
        Conversation? conversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.User2Id == user2Id &&
                c.ServiceId == serviceId
            );

        if (conversation == null)
        {
            return Response<Conversation?>.Failure("Conversación no encontrada.");
        }

        return Response<Conversation?>.Success(conversation, "Conversación obtenida con éxito.");
    }

    public async Task<Response<Conversation>> CreateConversationAsync(Guid userId, Guid user2Id, Guid serviceId)
    {
        Conversation conversation = new Conversation
        {
            UserId = userId,
            User2Id = user2Id,
            ServiceId = serviceId
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        
        return Response<Conversation>.Success(conversation, "Conversación creada con éxito.");
    }
}