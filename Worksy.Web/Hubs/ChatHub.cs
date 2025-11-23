using Microsoft.AspNetCore.SignalR;

namespace Worksy.Web.Hubs;

public class ChatHub: Hub
{
    public override Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var conversationId = http?.Request.Query["conversationId"];

        if (!string.IsNullOrEmpty(conversationId))
        {
            Groups.AddToGroupAsync(Context.ConnectionId, conversationId!);
        }

        return base.OnConnectedAsync();
    }
}