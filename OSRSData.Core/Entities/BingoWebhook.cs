using System;

namespace OSRSData.Core.Entities;

public class BingoWebhook
{
    public Guid Id { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
