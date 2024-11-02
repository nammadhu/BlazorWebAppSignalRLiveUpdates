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
    public ActionResult<FullData> GetBusinessCards(int townId)
    {
        if (TownHub._businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            return Ok(new FullData
            {
                VerifiedCardList = businessCards.VerifiedCardList,
                DraftCardList = businessCards.DraftCardList
            });
        }
        return NotFound();
    }

    [HttpGet("delta/{townId}/{lastSyncTime}")]
    public ActionResult<FullData> GetDeltaBusinessCards(int townId, DateTime lastSyncTime)
    {
        if (TownHub._businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var deltaData = new FullData
            {
                VerifiedCardList = businessCards.VerifiedCardList.Where(bc => bc.LastUpdated > lastSyncTime).ToList(),
                DraftCardList = businessCards.DraftCardList.Where(bc => bc.LastUpdated > lastSyncTime).ToList()
            };
            return Ok(deltaData);
        }
        return NotFound();
    }
}
