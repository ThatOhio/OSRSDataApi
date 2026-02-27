using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoConfigResponseDto
{
    [JsonPropertyName("webhooks")]
    public List<string> Webhooks { get; set; } = new();

    [JsonPropertyName("items")]
    public List<BingoItemDto> Items { get; set; } = new();

    [JsonPropertyName("teamConfig")]
    public BingoTeamConfigDto? TeamConfig { get; set; }
}

public class BingoTeamConfigDto
{
    [JsonPropertyName("teamName")]
    public string TeamName { get; set; } = string.Empty;

    [JsonPropertyName("teamNameColor")]
    public string TeamNameColor { get; set; } = string.Empty;

    [JsonPropertyName("dateTimeColor")]
    public string DateTimeColor { get; set; } = string.Empty;

    [JsonPropertyName("teamIcon")]
    public string TeamIcon { get; set; } = string.Empty;
}

public class BingoItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
