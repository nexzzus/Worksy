using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers
{
    public class ConversationsController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<IActionResult> Open(Guid prestadorId, Guid serviceId)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Buscar conversación existente
            var existing = await _conversationService
                .FindBetweenUsersAsync(userId, prestadorId, serviceId);

            if (existing != null && existing.Result != null)
            {
                return RedirectToAction("Chat", "Message", new
                {
                    conversationId = existing.Result.Id
                });
            }

            // Crear nueva conversación
            var created = await _conversationService
                .CreateConversationAsync(prestadorId, serviceId);


            if (!created.isSuccess || created.Result == null)
            {
                return BadRequest("No se pudo crear la conversación.");
            }

            return RedirectToAction("Chat", "Message", new
            {
                conversationId = created.Result.Id
            });
        }

        public async Task<IActionResult> Index()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var chats = _conversationService.GetConversationsAsync(userId);
            if (chats.IsFaulted)
            {
                return BadRequest("No se pudieron cargar las conversaciones.");
            }

            ViewBag.Chats = chats.Result.Result;
            
            return View();
        }
    }
}