namespace OSRSData.Core.Entities;

public class LootItem
{
    public Guid Id { get; set; }
    public int ItemId { get; set; } // The item ID from RuneLite
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Price { get; set; }
    
    public Guid LootRecordId { get; set; }
    public LootRecord LootRecord { get; set; } = null!;
}
