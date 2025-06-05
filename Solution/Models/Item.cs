using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class Item
{
    // Constructor used when creating a new item
    public Item(string itemName, string itemIcon, int itemCount, int itemCost, string itemDescription,
        int popularity = 0)
    {
        ItemName = itemName;
        ItemIcon = itemIcon;
        ItemCost = itemCost;
        ItemDescription = itemDescription;
        Popularity = popularity;
    }

    // Required by MongoDB for deserialization
    public Item()
    {
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")] public string ItemName { get; set; }

    [BsonElement("icon")] public string ItemIcon { get; set; }

    [BsonElement("cost")] public int ItemCost { get; set; }

    [BsonElement("description")] public string ItemDescription { get; set; }

    [BsonElement("popularity")] public int Popularity { get; set; }
}