using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoWebhookUpdateDto
{
    [JsonPropertyName("characterName")]
    public string CharacterName { get; set; } = string.Empty;

    [JsonPropertyName("webhookUrl")]
    public string WebhookUrl { get; set; } = string.Empty;
}
