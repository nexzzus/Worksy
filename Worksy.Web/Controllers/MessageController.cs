using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Worksy.Web.Core;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Hubs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class MessageController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IConversationService _conversationService;
    private readonly IHubContext<ChatHub> _hubContext;

    private readonly INotyfService _notyfService;

    // GET
    public MessageController(IMessageService messageService, IConversationService conversationService,
        INotyfService notyfService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _conversationService = conversationService;
        _notyfService = notyfService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Chat(Guid conversationId)
    {
        Response<Conversation?> conversation = await _conversationService.GetConversationAsync(conversationId);
        if (!conversation.isSuccess || conversation.Result == null)
        {
            _notyfService.Error("La conversación no existe.");
            return NotFound();
        }

        Response<List<Message>> messages = await _messageService.GetMessagesAsync(conversationId);
        if (!messages.isSuccess)
        {
            _notyfService.Error(messages.Message);
            return View();
        }

        ViewBag.ConversationId = conversationId;
        return View(messages.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Send(Guid conversationId, string content)
    {
        Guid userId = Guid.Parse(User.FindFirst("UserId")!.Value);

        if (string.IsNullOrWhiteSpace(content))
        {
            _notyfService.Error("El mensaje no puede estar vacío.");
            return RedirectToAction("Chat", new { conversationId });
        }

        var response = await _messageService.AddMessageAsync(conversationId, userId, content);

        if (!response.isSuccess)
        {
            _notyfService.Error(response.Message);
            return RedirectToAction("Chat", new { conversationId });
        }

        // Enviar mensaje por SignalR a TODOS menos al emisor
        await _hubContext.Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage",
                userId.ToString(),
                content,
                response.Result.SentAt,
                conversationId);

        return RedirectToAction("Chat", new { conversationId });
    }

}