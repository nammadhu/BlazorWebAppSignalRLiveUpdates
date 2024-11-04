using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static Shared.iCardDto;

namespace BlazorWebAppSignalR.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TownController : ControllerBase
{
    //private static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();

    [HttpGet("{townId}")]
    public ActionResult<TownCardsDto> GetBusinessCards(int townId)
    {
        if (TownSignalRHub._businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            return Ok(new TownCardsDto
            {
                VerifiedCards = businessCards.VerifiedCardList,
                DraftCards = businessCards.DraftCardList
            });
        }
        return NotFound();
    }

    [HttpGet("delta/{townId}/{lastSyncTime}")]
    public ActionResult<TownCardsDto> GetDeltaBusinessCards(int townId, DateTime lastSyncTime)
    {
        if (TownSignalRHub._businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var deltaData = new TownCardsDto
            {
                VerifiedCards = businessCards.VerifiedCardList.Where(bc => bc.LastUpdated > lastSyncTime).ToList(),
                DraftCards = businessCards.DraftCardList.Where(bc => bc.LastUpdated > lastSyncTime).ToList()
            };
            return Ok(deltaData);
        }
        return NotFound();
    }
}
