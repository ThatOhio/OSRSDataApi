using System;

namespace OSRSData.Core.Entities;

public class BingoItem
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Source { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
