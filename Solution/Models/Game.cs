using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class Game
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")] 
    public string Name { get; set; } = string.Empty;

    [BsonElement("money")] 
    public int Money { get; set; }

    [BsonElement("inventory")] 
    public List<InventoryEntry> Inventory { get; set; } = new();

    [BsonElement("grid")] 
    public List<List<string?>> Grid { get; set; } = new();
}