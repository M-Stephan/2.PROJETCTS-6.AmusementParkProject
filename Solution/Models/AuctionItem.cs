using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class AuctionItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    public string? Id { get; set; }

    [BsonElement("item_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; }

    [BsonElement("seller_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SellerGameId { get; set; } = null!;

    [BsonElement("buyer_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? BuyerGameId { get; set; }

    [BsonElement("price")] public int Price { get; set; }

    [BsonElement("quantity")] public int Quantity { get; set; }

    [BsonElement("is_sold")] public bool IsSold { get; set; }

    [BsonElement("created_at")] public DateTime CreatedAt { get; set; }
}