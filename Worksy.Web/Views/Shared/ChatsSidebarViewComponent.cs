using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Views.Shared;

public class ChatsSidebarViewComponent: ViewComponent
{
    private readonly IConversationService _conversationService;

    public ChatsSidebarViewComponent(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Obtener usuario logueado
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
            return View("Default", Enumerable.Empty<Conversation>());

        Guid userId = Guid.Parse(idClaim.Value);

        var chats = await _conversationService.GetConversationsAsync(userId);
        

        return View("Default", chats.Result);
    }
}