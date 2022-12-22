using Microsoft.AspNetCore.SignalR;

namespace server.Hubs
{
    public class Loghub : Hub
    {
        public async Task SendMessage(string user, string state, string time)
        {
            await Clients.All.SendAsync("Locktoggled", user, state, time);
        }
    }
}