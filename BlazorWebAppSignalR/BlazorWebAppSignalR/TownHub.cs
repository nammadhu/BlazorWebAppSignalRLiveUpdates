namespace BlazorWebAppSignalR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;

public class TownHub : Hub
{
    //internal static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();
    internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList)> _businessCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>)>();

    public async Task JoinGroup(int townId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, townId.ToString());
    }

    public async Task LeaveGroup(int townId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId.ToString());
    }

    //[Authorize]
    public async Task AddBusinessCard(int townId, iCardDto businessCardDto, bool isVerified)
    {
        //todo if role admin then only verified
        /*
         var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (isVerified && !roles.Contains("Admin"))
        {
            throw new HubException("Only admins can add verified cards.");
        }
        */

        var businessCards = _businessCardsDictionary.GetOrAdd(townId, (new List<iCardDto>(), new List<iCardDto>()));
        businessCardDto.LastUpdated = DateTime.UtcNow;

        if (isVerified)
        {
            businessCards.VerifiedCardList.Add(businessCardDto);
        }
        else
        {
            businessCards.DraftCardList.Add(businessCardDto);
        }

        // Broadcast the new business card to all clients in the group
        await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
    }

    //[Authorize]
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

                // Broadcast the updated business card to all clients in the group
                await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
            }
        }
    }
}