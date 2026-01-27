namespace OSRSData.Core.Entities;

public class DeathRecord
{
    public Guid Id { get; set; }
    public int RegionId { get; set; }
    public string? Killer { get; set; }
}
