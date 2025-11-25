using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly DataContext _context;

    public MessageService(DataContext context)
    {
        _context = context;
    }

    public async Task<Response<List<Message>>> GetMessagesAsync(Guid conversationId)
    {
        var message = _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        if (!message.Result.Any())
        {
            return Response<List<Message>>.Failure("No se encontraron mensajes para esta conversación.");
        }

        return Response<List<Message>>.Success(await message, "Mensajes obtenidos con éxito.");
    }

    public async Task<Response<Message>> AddMessageAsync(Guid conversationId, Guid senderId, string content)
    {
        // 1. Validar senderId vacío
        if (senderId == Guid.Empty)
            return Response<Message>.Failure("El remitente es inválido.");

        // 2. Verificar que la conversación exista
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            return Response<Message>.Failure("La conversación no existe.");

        bool userIsParticipant =
            conversation.CustomerId == senderId ||
            conversation.ProviderId == senderId;
        
        // 3. Validar que el usuario pertenezca a la conversación
        if (!userIsParticipant)
        {
            return Response<Message>.Failure("No puedes enviar mensajes en una conversación que no es tuya.");
        }

        // 4. Crear mensaje
        Message message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Response<Message>.Success(message, "Mensaje enviado correctamente.");
    }

}