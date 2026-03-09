namespace OSRSData.App.DTOs;

public class PlayerLootValueDto
{
    public string CharacterName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public long TotalLootValue { get; set; }
}

public class TeamLootValueDto
{
    public string TeamName { get; set; } = string.Empty;
    public long TotalLootValue { get; set; }
}
