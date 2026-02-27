using System;

namespace OSRSData.Core.Entities;

public class BingoTeamConfig
{
    public Guid Id { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string TeamNameColor { get; set; } = string.Empty;
    public string DateTimeColor { get; set; } = string.Empty;
    public string TeamIcon { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
