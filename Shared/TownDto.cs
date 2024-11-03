namespace Shared;

public class TownDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class iCardDto
{
    public bool IsVerified { get; set; }
    public string Id { get; set; }
    public string BusinessName { get; set; }
    public string ContactName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public int TownId { get; set; }
    public DateTime LastUpdated
    {
        get; set;
    }
    // Add other business card properties as needed

    public class TownCardsDto
    {
        public List<iCardDto>? VerifiedCards { get; set; }
        public List<iCardDto>? DraftCards { get; set; }
        public DateTime LastSyncedTime { get; set; }
    }

}