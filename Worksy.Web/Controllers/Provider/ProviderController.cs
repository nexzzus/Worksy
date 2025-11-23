using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers.Provider;

public class ProviderController : Controller
{
    private readonly IServicesService _servicesService;
    private readonly IConversationService _conversation;
    
    public ProviderController(IServicesService providerService, IConversationService conversation)
    {
        _servicesService = providerService;
        _conversation = conversation;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var services = await _servicesService.GetServicesByProviderAsync(providerId);
        var chats = await _conversation.GetConversationsByProviderAsync(providerId);

        ViewBag.Chats = chats.Result;
        return View(services.Result);
    }
}