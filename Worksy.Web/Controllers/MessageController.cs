using System.Security.Claims;
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
    private readonly IUserService _userService;

    public MessageController(
        IMessageService messageService,
        IConversationService conversationService,
        INotyfService notyfService,
        IHubContext<ChatHub> hubContext, IUserService userService)
    {
        _messageService = messageService;
        _conversationService = conversationService;
        _notyfService = notyfService;
        _hubContext = hubContext;
        _userService = userService;
    }


    public async Task<IActionResult> Chat(Guid conversationId)
    {
        var conversation = await _conversationService.GetConversationAsync(conversationId);

        if (!conversation.isSuccess || conversation.Result == null)
        {
            _notyfService.Error("La conversación no existe.");
            return NotFound();
        }

        var messages = await _messageService.GetMessagesAsync(conversationId);

        if (!messages.isSuccess)
        {
            _notyfService.Success(messages.Message);
            ViewBag.ConversationId = conversationId;

            return View(new List<Message>());
        }

        ViewBag.ConversationId = conversationId;
        
        var currentRol = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN, Env.ROLE_COLLAB]);
        ViewData["Layout"] = currentRol
            ? "_Dashboard"
            : "_Layout";

        return View(messages.Result);
    }


    [HttpPost]
    public async Task<IActionResult> Send(Guid conversationId, string content)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

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
        
        await _hubContext.Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                conversationId,
                userId = userId.ToString(),
                message = content,
                sentAt = response.Result.SentAt
            });
        
        return RedirectToAction("Chat", new { conversationId });
    }
}