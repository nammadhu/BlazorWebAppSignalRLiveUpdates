namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

//internal static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();
//internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList)> _businessCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>)>();
public class TownSignalRHub : Hub
{
    internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList, int UserCount, DateTime LastAccessed)> _businessCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>, int, DateTime)>();

    private static readonly Timer cleanupTimer;
    static TownSignalRHub()
    {
        cleanupTimer = new Timer(60000); // Check every minute
        cleanupTimer.Elapsed += CleanupExpiredEntries;
        cleanupTimer.Start();
    }
    public TownSignalRHub()//for service related instantiation
    { 
    
    }
    //NOTE: DOnt use cancellation token here at any methods like JoinGroup or LeaveGroup, it wont work
    public async Task JoinGroup(int townId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, townId.ToString());

        _businessCardsDictionary.AddOrUpdate(townId,
            (new List<iCardDto>(), new List<iCardDto>(), 1, DateTime.UtcNow),
            (key, value) => (value.VerifiedCardList, value.DraftCardList, value.UserCount + 1, DateTime.UtcNow));
    }

    public async Task LeaveGroup(int townId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId.ToString());

        if (_businessCardsDictionary.TryGetValue(townId, out var entry))
        {
            entry.UserCount -= 1;
            if (entry.UserCount == 0)
            {
                _businessCardsDictionary.TryRemove(townId, out _);
            }
            else
            {
                _businessCardsDictionary[townId] = entry;
            }
        }
    }

    public async Task AddBusinessCard(int townId, iCardDto businessCardDto, bool isVerified)
    {
        var businessCards = _businessCardsDictionary.GetOrAdd(townId, (new List<iCardDto>(), new List<iCardDto>(), 0, DateTime.UtcNow));
        businessCardDto.LastUpdated = DateTime.UtcNow;

        if (isVerified)
        {
            businessCards.VerifiedCardList.Add(businessCardDto);
        }
        else
        {
            businessCards.DraftCardList.Add(businessCardDto);
        }

        businessCards.LastAccessed = DateTime.UtcNow;
        _businessCardsDictionary[townId] = businessCards;

        // Broadcast the new business card to all clients in the group
        await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
    }

    public async Task UpdateBusinessCard(int townId, iCardDto businessCardDto, bool isVerified)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var cardList = isVerified ? businessCards.VerifiedCardList : businessCards.DraftCardList;
            var index = cardList.FindIndex(bc => bc.Id == businessCardDto.Id);

            if (index >= 0)
            {
                businessCardDto.LastUpdated = DateTime.UtcNow;
                cardList[index] = businessCardDto;

                businessCards.LastAccessed = DateTime.UtcNow;
                _businessCardsDictionary[townId] = businessCards;

                // Broadcast the updated business card to all clients in the group
                await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
            }
        }
    }

    private static void CleanupExpiredEntries(object? sender, ElapsedEventArgs e)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-10); // Entries older than 10 minutes will be removed

        foreach (var key in _businessCardsDictionary.Keys)
        {
            if (_businessCardsDictionary.TryGetValue(key, out var entry) && entry.LastAccessed < expirationTime)
            {
                _businessCardsDictionary.TryRemove(key, out _);
            }
        }
    }
}