namespace OSRSData.Core.Entities;

public class LootRecord
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public long TotalValue { get; set; }
    public int? Kc { get; set; }
    
    public ICollection<LootItem> Items { get; set; } = new List<LootItem>();
}
