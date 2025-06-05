using dotenv.net;
using MongoDB.Driver;
using Solution.Models;

namespace Solution.Data;

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
        _database = _client.GetDatabase("park"); // Use your actual database name
    }

    public IMongoCollection<Game> GetGameCollection()
    {
        return _database.GetCollection<Game>("games");
    }

    public IMongoCollection<Item> GetItemCollection()
    {
        return _database.GetCollection<Item>("items");
    }

    public IMongoCollection<AuctionItem> GetAuctionCollection()
    {
        return _database.GetCollection<AuctionItem>("auctions");
    }

    public List<Game> GetAllGames()
    {
        return GetGameCollection().Find(_ => true).ToList();
    }

    public Game? GetGameByNickname(string name)
    {
        return GetGameCollection().Find(game => game.Name == name).FirstOrDefault();
    }

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

    public void DeleteGame(string name)
    {
        var collection = GetGameCollection();
        var filter = Builders<Game>.Filter.Eq(g => g.Name, name);
        collection.DeleteOne(filter);
    }
}