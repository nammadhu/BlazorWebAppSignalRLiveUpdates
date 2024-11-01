namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class TownHub : Hub
{
    private static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();

    public async Task JoinGroup(string townId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, townId);
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            await Clients.Caller.SendAsync("ReceiveInitialBusinessCards", businessCards);
        }
        else
        {
            _businessCardsDictionary[townId] = new List<BusinessCardDto>();
            await Clients.Caller.SendAsync("ReceiveInitialBusinessCards", new List<BusinessCardDto>());
        }
    }

    public async Task LeaveGroup(string townId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId);
    }

    public async Task AddBusinessCard(string townId, BusinessCardDto businessCardDto)
    {
        var businessCards = _businessCardsDictionary.GetOrAdd(townId, new List<BusinessCardDto>());
        businessCardDto.LastUpdated = DateTime.UtcNow;
        businessCards.Add(businessCardDto);
        // Broadcast the new business card to all clients in the group
        await Clients.Group(townId).SendAsync("ReceiveBusinessCard", businessCardDto);
    }

    public async Task UpdateBusinessCard(string townId, BusinessCardDto businessCardDto)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var index = businessCards.FindIndex(bc => bc.Id == businessCardDto.Id);
            if (index >= 0)
            {
                {
                    businessCardDto.LastUpdated = DateTime.UtcNow;
                    businessCards[index] = businessCardDto;

                    // Broadcast the updated business card to all clients in the group
                    await Clients.Group(townId).SendAsync("ReceiveBusinessCard", businessCardDto);
                }
            }
        }
    }
    public async Task<List<BusinessCardDto>> GetDeltaUpdates(string townId, DateTime lastUpdated)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var deltaUpdates = businessCards.Where(bc => bc.LastUpdated > lastUpdated).ToList();
            return deltaUpdates;
        }
        return new List<BusinessCardDto>();
    }

}