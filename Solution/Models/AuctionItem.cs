using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Solution.Models;

public class AuctionItem
{
    [BsonId] public ObjectId Id { get; set; }

    public string ItemId { get; set; } = null!;
    public string SellerName { get; set; } = null!;
    public int Price { get; set; } 
    public int Quantity { get; set; } 
    public string? BuyerName { get; set; } 
    public bool IsSold { get; set; } 
    public DateTime CreatedAt { get; set; }
}