using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class InventoryEntry
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; } = string.Empty;
    public int Count { get; set; }
}