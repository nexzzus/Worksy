using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers.Provider;

public class ChatsProviderController : Controller
{
    private readonly IConversationService _conversationService;

    public ChatsProviderController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }
    
    public async Task<IActionResult> Index()
    {
        var providerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var chats = await _conversationService.GetConversationsByProviderAsync(providerId);

        return RedirectToPage("Provider/Chats/Index", chats.Result);
    }
}