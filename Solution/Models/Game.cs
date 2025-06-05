using MongoDB.Bson;

namespace Solution.Models;

public class Game
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Money { get; set; }
    public List<InventoryEntry> Inventory { get; set; } = new();
    public List<List<string?>> Grid { get; set; } = new();
}