using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebAppSignalR.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TownController : ControllerBase
{
    private static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();

    [HttpGet("{townId}")]
    public ActionResult<List<BusinessCardDto>> GetBusinessCards(string townId)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            return Ok(businessCards);
        }
        return NotFound();
    }

    [HttpGet("delta/{townId}/{lastSyncTime}")]
    public ActionResult<List<BusinessCardDto>> GetDeltaBusinessCards(string townId, DateTime lastSyncTime)
    {
        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var deltaUpdates = businessCards.Where(bc => bc.LastUpdated > lastSyncTime).ToList();
            return Ok(deltaUpdates);
        }
        return NotFound();
    }
}
