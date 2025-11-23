using Microsoft.AspNetCore.SignalR;

namespace Worksy.Web.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var conversationId = http?.Request.Query["conversationId"].ToString();

            if (!string.IsNullOrEmpty(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string conversationId, string userId, string message)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
            {
                conversationId,
                userId,
                message,
                sentAt = DateTime.UtcNow
            });
        }
    }
}