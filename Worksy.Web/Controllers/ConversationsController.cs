using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class ConversationsController : Controller
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    // GET
    public async Task<IActionResult> Open(Guid prestadorId, Guid serviceId)
    {
        // Usuario autenticado
        Guid userId = Guid.Parse(User.FindFirst("UserId")!.Value);

        // Verificar si ya existe una conversación entre el usuario y el prestador para el servicio dado
        var existing = await _conversationService
            .FindBetweenUsersAsync(userId, prestadorId, serviceId);

        if (existing.isSuccess)
        {
            return RedirectToAction("Chat", "Message", new {conversationId = existing.Result.Id});
        }
        
        // Crear conversación nueva
        var conversation = await _conversationService
            .CreateConversationAsync(userId, prestadorId, serviceId);
        
        return RedirectToAction("Chat", "Message", new{ conversationId = existing.Result.Id});
    }
}