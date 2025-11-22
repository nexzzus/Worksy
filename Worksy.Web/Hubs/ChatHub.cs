using Microsoft.AspNetCore.SignalR;

namespace Worksy.Web.Hubs;

public class ChatHub: Hub
{
    public async Task SendMessage(Guid fromUser, Guid toUser, string message)
    {
        await Clients.User(toUser.ToString()).SendAsync("ReceiveMessage", fromUser, message);
    }
}