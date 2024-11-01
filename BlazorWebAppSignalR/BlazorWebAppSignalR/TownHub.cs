namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Collections.Concurrent;
using System.Threading.Tasks;
public class TownHub : Hub
{
    // Dictionary to maintain lists of business cards for each town
    private static ConcurrentDictionary<string, List<BusinessCard>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCard>>();

    public override async Task OnConnectedAsync()
    {
        // No need to send the list on connection
    }
    public async Task JoinGroup(string townId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, townId);
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            await Clients.Caller.SendAsync("ReceiveBusinessCards", businessCards);
        }
        else
        {
            _businessCardsDictionary[townId] = new List<BusinessCard>();
            await Clients.Caller.SendAsync("ReceiveBusinessCards", new List<BusinessCard>());
        }
    }

    public async Task LeaveGroup(string townId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId);
    }
    public async Task AddBusinessCard(string townId, BusinessCard businessCardDto)
    {
        var businessCards = _businessCardsDictionary.GetOrAdd(townId, new List<BusinessCard>());
        businessCards.Add(businessCardDto);

        // Broadcast the updated list to all clients in the group
        await Clients.Group(townId).SendAsync("ReceiveBusinessCards", businessCards);
    }

    public async Task UpdateBusinessCard(string townId, BusinessCard businessCardDto)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var index = businessCards.FindIndex(bc => bc.Id == businessCardDto.Id);
            if (index >= 0)
            {
                businessCards[index] = businessCardDto;

                // Broadcast the updated list to all clients in the group
                await Clients.Group(townId).SendAsync("ReceiveBusinessCards", businessCards);
            }
        }

    }
}
