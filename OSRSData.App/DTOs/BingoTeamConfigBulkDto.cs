using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoTeamConfigBulkDto : BingoTeamConfigUpdateDto
{
    [JsonPropertyName("characterName")]
    public string CharacterName { get; set; } = string.Empty;
}
