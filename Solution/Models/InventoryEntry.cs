using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class InventoryEntry
{
    [BsonElement("item_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; } = string.Empty;

    [BsonElement("count")] public int Count { get; set; }
}