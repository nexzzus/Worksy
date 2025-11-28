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
            .Include(c => c.Customer)
            .Include(c => c.Provider)
            .Include(c => c.Service)
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
            .Include(c => c.Messages)
            .Include(c => c.Customer)
            .Include(c => c.Provider)
            .FirstOrDefaultAsync(c =>
                (
                    (c.CustomerId == userId && c.ProviderId == user2Id) ||
                    (c.CustomerId == user2Id && c.ProviderId == userId)
                )
                && c.ServiceId == serviceId
            );
        if (conversation == null)
        {
            return Response<Conversation?>.Failure("Conversación no encontrada.");
        }

        return Response<Conversation?>.Success(conversation, "Conversación obtenida con éxito.");
    }

    public async Task<Response<Conversation>> CreateConversationAsync(Guid customerId, Guid serviceId)
    {
        // Obtener el proveedor REAL del servicio
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId);

        if (service == null)
            return Response<Conversation>.Failure("El servicio no existe.");

        var providerId = service.UserId;

        if (providerId == Guid.Empty)
        {
            return Response<Conversation>.Failure("El servicio no tiene un proveedor asignado.");
        }
        
        Conversation conversation = new Conversation
        {
            CustomerId = customerId,
            ProviderId = (Guid)providerId!,
            ServiceId = serviceId
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return Response<Conversation>.Success(conversation, "Conversación creada con éxito.");
    }

    public async Task<Response<List<Conversation>>> GetConversationsByProviderAsync(Guid providerId)
    {
        var conversations = await _context.Conversations
            .Where(c => c.ProviderId == providerId)
            .Include(c => c.Customer)
            .Include(c => c.Service)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return Response<List<Conversation>>.Success(conversations);
    }

    public async Task<Response<List<Conversation>>> GetConversationsAsync(Guid userId)
    {
        try
        {
            List<Conversation> conversations = await _context.Conversations
                .Include(c => c.Customer)
                .Include(c => c.Provider)
                .Where(c => c.CustomerId == userId || c.ProviderId == userId)
                .OrderByDescending(c=> c.CreatedAt)
                .ToListAsync();

            if (conversations.Count == 0)
            {
                return Response<List<Conversation>>.Failure("No se encontraron conversaciones para el usuario.");
            }
            
            return Response<List<Conversation>>.Success(conversations, "Conversaciones obtenidas con éxito.");
        }
        catch (Exception e)
        {
            return Response<List<Conversation>>.Failure(e, "Error al obtener las conversaciones.");
        }
    }
}