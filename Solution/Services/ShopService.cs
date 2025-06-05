using MongoDB.Driver;
using Solution.Models;
using Spectre.Console;

namespace Solution.Services;

public class ShopService
{
    private readonly IMongoCollection<Item> _itemCollection;

    public ShopService(IMongoCollection<Item> itemCollection)
    {
        _itemCollection = itemCollection;
    }

    public void BuyMenu(InventoryService inventoryService, BankingService money)
    {
        var allItems = _itemCollection.Find(FilterDefinition<Item>.Empty).ToList();

        // Add a "Back to main menu" dummy option by allowing null selection
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<Item?>()
                .Title("[underline green]Choose an attraction to buy[/]")
                .PageSize(10)
                .AddChoices(allItems.Cast<Item?>().Append(null))
                .UseConverter(item => item == null
                    ? "[bold white]\u2b05\ufe0f Back to the main menu[/]"
                    : $"[]{item.ItemIcon}[/] [bold yellow]{item.ItemName}[/] [green](${item.ItemCost})[/] [blue](Popularity: {item.Popularity})[/] - [grey]{item.ItemDescription}[/]")
        );

        if (choice == null)
            return;

        var item = choice;

        if (item.ItemCost <= money._money)
        {
            inventoryService.AddItem(item.Id!); // ✅ Add by ID
            money.RemoveMoney(item.ItemCost);

            // Increase popularity by 1
            var filter = Builders<Item>.Filter.Eq(i => i.Id, item.Id);
            var update = Builders<Item>.Update.Inc(i => i.Popularity, 1);
            _itemCollection.UpdateOne(filter, update);

            AnsiConsole.MarkupLine($"[green]You bought {item.ItemName} for ${item.ItemCost}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("\n[red]You don't have enough money.[/]");
        }

        //AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
        Console.ReadKey(true);
    }


    public void SellMenu(InventoryService inventoryService, BankingService money)
    {
        var entries = inventoryService.Entries;

        if (entries.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Your inventory is empty![/]\n");
            // AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
            Console.ReadKey(true);
            return;
        }

        var itemsToSell = new List<(string DisplayName, string ItemId, int Price)>();

        foreach (var entry in entries)
        {
            var item = _itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();
            if (item != null)
                itemsToSell.Add(($"{item.ItemName} (x{entry.Count}) - Sell for ${item.ItemCost}", item.Id!,
                    item.ItemCost));
        }

        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[green]Choose an item to sell[/]")
            .AddChoices(itemsToSell.Select(x => x.DisplayName)));

        var selected = itemsToSell.FirstOrDefault(x => x.DisplayName == choice);

        if (!string.IsNullOrEmpty(selected.ItemId))
        {
            inventoryService.RemoveItem(selected.ItemId);
            money.AddMoney(selected.Price);
            AnsiConsole.MarkupLine($"[green]You sold an item for ${selected.Price}[/]");
        }

        //AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
        Console.ReadKey(true);
    }
}