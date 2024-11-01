namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Threading.Tasks;
public class TownsHub : Hub
{
    private static List<TownDto> _towns = new List<TownDto>();

    public override async Task OnConnectedAsync()
    {
        // Send the current list to the newly connected user
        await Clients.Caller.SendAsync("ReceiveMessage", _towns);
    }

    public async Task AddTown(TownDto townDto)
    {
        // Add new entry to the list
        _towns.Add(townDto);

        // Broadcast the updated list to all clients
        await Clients.All.SendAsync("ReceiveMessage", _towns);
    }

    /*
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
    */
}
