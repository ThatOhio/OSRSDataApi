using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class LogEntryDto
{
    [JsonPropertyName("player")]
    public string? Player { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("data")]
    public LootRecordDto Data { get; set; } = null!;
}

public class LootRecordDto
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<LootItemDto> Items { get; set; } = new();

    [JsonPropertyName("totalValue")]
    public long TotalValue { get; set; }

    [JsonPropertyName("kc")]
    public int? Kc { get; set; }
}

public class LootItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }
}
