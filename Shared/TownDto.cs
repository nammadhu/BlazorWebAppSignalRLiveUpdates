namespace Shared;

public class TownDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class BusinessCardDto
{
    public string Id { get; set; }
    public string BusinessName { get; set; }
    public string ContactName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public string TownId { get; set; }
    public DateTime LastUpdated
    {
        get; set;
    }
    // Add other business card properties as needed
}