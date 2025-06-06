using dotenv.net;
using MongoDB.Driver;
using Solution.Models;

namespace Solution.Services;

public class MongoDbService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoDbService()
    {
        DotEnv.Load();
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString),
                "MongoDB connection string not found in environment variables.");

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("park");
    }

    // Collections
    public IMongoCollection<Game> GetGameCollection() => _database.GetCollection<Game>("games");
    public IMongoCollection<Item> GetItemCollection() => _database.GetCollection<Item>("items");
    public IMongoCollection<AuctionItem> GetAuctionCollection() => _database.GetCollection<AuctionItem>("auctions");

    // Game retrieval
    public List<Game> GetAllGames() => GetGameCollection().Find(_ => true).ToList();

    public Game? GetGameById(string id) =>
        GetGameCollection().Find(game => game.Id == id).FirstOrDefault();

    public Game? GetGameByNickname(string name) =>
        GetGameCollection().Find(game => game.Name == name).FirstOrDefault();

    // Game save or update
    public void SaveGame(Game game)
    {
        var collection = GetGameCollection();
        var filter = Builders<Game>.Filter.Eq(g => g.Name, game.Name);
        var existing = collection.Find(filter).FirstOrDefault();

        if (existing == null)
        {
            collection.InsertOne(game);
        }
        else
        {
            game.Id = existing.Id;
            collection.ReplaceOne(filter, game);
        }
    }

    // Deletion
    public void DeleteGame(string name)
    {
        var collection = GetGameCollection();
        var filter = Builders<Game>.Filter.Eq(g => g.Name, name);
        collection.DeleteOne(filter);
    }

    public void DeleteUnsoldAuctionItemsBySellerId(string sellerGameId)
    {
        var filter = Builders<AuctionItem>.Filter.And(
            Builders<AuctionItem>.Filter.Eq(ai => ai.SellerGameId, sellerGameId),
            Builders<AuctionItem>.Filter.Eq(ai => ai.IsSold, false)
        );
        GetAuctionCollection().DeleteMany(filter);
    }

}
