using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class MessageController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IConversationService _conversationService;

    private readonly INotyfService _notyfService;

    // GET
    public MessageController(IMessageService messageService, IConversationService conversationService,
        INotyfService notyfService)
    {
        _messageService = messageService;
        _conversationService = conversationService;
        _notyfService = notyfService;
    }

    public async Task<IActionResult> Chat(Guid conversationId)
    {
        Response<Conversation?> conversation = await _conversationService.GetConversationAsync(conversationId);
        if (conversation is null)
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
        Guid useId = Guid.Parse(User.FindFirst("UserId")?.Value!);
        if (useId == Guid.Empty)
        {
            _notyfService.Error("Usuario no autenticado.");
            return RedirectToAction("Chat", new { conversationId });
        }

        Response<Message> response = await _messageService.AddMessageAsync(conversationId, useId, content);

        if (!response.isSuccess)
        {
            _notyfService.Error(response.Message);
            return RedirectToAction("Chat", new { conversationId });
        }

        _notyfService.Success("Mensaje enviado con éxito.");
        return Ok(response.Result);
    }
}