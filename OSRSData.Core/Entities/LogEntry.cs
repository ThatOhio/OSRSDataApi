using OSRSData.Core.Enums;

namespace OSRSData.Core.Entities;

public class LogEntry
{
    public Guid Id { get; set; }
    public string? Player { get; set; }
    public LogType Type { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // For now, we only have LootRecord. 
    // In the future we might have different types of data.
    public Guid? LootRecordId { get; set; }
    public LootRecord? LootRecord { get; set; }

    public Guid? DeathRecordId { get; set; }
    public DeathRecord? DeathRecord { get; set; }
}
