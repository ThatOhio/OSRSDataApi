using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoConfigResponseDto
{
    [JsonPropertyName("webhooks")]
    public List<string> Webhooks { get; set; } = new();

    [JsonPropertyName("items")]
    public List<BingoItemDto> Items { get; set; } = new();
}

public class BingoItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
