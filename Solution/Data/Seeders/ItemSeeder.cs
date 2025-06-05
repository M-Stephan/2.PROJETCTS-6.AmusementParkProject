using MongoDB.Driver;
using Solution.Models;

// For MongoDbService

namespace Solution.Data.Seeders;

public class ItemSeeder
{
    private readonly IMongoCollection<Item> _itemCollection;

    public ItemSeeder(MongoDbService mongoDbService)
    {
        // Get the "items" collection via MongoDbService
        _itemCollection = mongoDbService.GetItemCollection();
    }

    public void SeedItems()
    {
        var knownItems = new List<Item>
        {
            new(
                "Ferris wheel",
                "[gold1]🎡[/]",
                1,
                25060,
                "A slow, scenic ride with a great view from the top."),

            new(
                "Roller Coaster",
                "[red]🎢[/]",
                1,
                15170,
                "High-speed thrills and heart-racing drops."),

            new(
                "Carousel",
                "[orchid1]🎠[/]",
                1,
                7550,
                "A classic spinning ride for all ages."),

            new(
                "Food Stand",
                "[yellow1]🌭[/]",
                1,
                3250,
                "Satisfy your hunger with snacks and drinks."),

            new(
                "Ticket Booth",
                "[blue]🎫[/]",
                1,
                7500,
                "Entry point to manage and sell tickets."),

            new(
                "Bumper Cars",
                "[orange1]🚗[/]",
                1,
                12320,
                "Drive, crash, and laugh with friends."),

            new(
                "Water Slide",
                "[deepskyblue1]🌊[/]",
                1,
                13480,
                "Slide down for a splashy good time."),

            new(
                "Swing Ride",
                "[violet]🎑[/]",
                1,
                12865,
                "Feel the wind as you spin through the air.")
        };

        foreach (var item in knownItems)
        {
            var exists = _itemCollection.Find(i => i.ItemName == item.ItemName).Any();
            if (!exists) _itemCollection.InsertOne(item);
        }
    }
}