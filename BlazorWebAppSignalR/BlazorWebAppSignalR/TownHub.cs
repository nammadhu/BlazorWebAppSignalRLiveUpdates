namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Threading.Tasks;
    public class TownHub : Hub
    {
        public async Task JoinGroup(string townId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, townId);
        }

        public async Task LeaveGroup(string townId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId);
        }

        public async Task SendMessageToGroup(string townId, TownDto townDto)
        {
            await Clients.Group(townId).SendAsync("ReceiveMessage", townDto);
        }
    }
