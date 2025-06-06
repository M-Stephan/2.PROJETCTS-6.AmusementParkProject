using MongoDB.Driver;
using Park;
using Solution.Data;
using Solution.Interfaces;
using Solution.Models;
using Spectre.Console;

namespace Solution.Services;

/// <summary>
/// Handles auction-related operations such as listing, retrieving, and buying auction items.
/// </summary>
public class AuctionService : IAuction
{
    private readonly IMongoCollection<AuctionItem> _auctionCollection;
    private readonly IMongoCollection<Item> _itemCollection;
    private readonly MongoDbService _mongoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionService"/> class.
    /// </summary>
    public AuctionService(IMongoCollection<AuctionItem> auctionCollection, IMongoCollection<Item> itemCollection,
        MongoDbService mongoService)
    {
        _auctionCollection = auctionCollection;
        _itemCollection = itemCollection;
        _mongoService = mongoService;
    }

    /// <summary>
    /// Retrieves all active (unsold) auction items.
    /// </summary>
    public List<AuctionItem> GetAllActiveItems()
    {
        return _auctionCollection.Find(x => !x.IsSold).ToList();
    }

    /// <summary>
    /// Retrieves all auction items available to buy for a specific game (excluding that game's own listings).
    /// </summary>
    public List<AuctionItem> GetAvailableItemsToBuy(string currentGameId)
    {
        return _auctionCollection.Find(x => !x.IsSold && x.SellerGameId != currentGameId).ToList();
    }

    /// <summary>
    /// Allows a player to list an item from their inventory for auction.
    /// </summary>
    public void ListItem(Game currentGame, InventoryService inventory)
    {
        AnsiConsole.Write(new Rule("[bold blue]List Item on Auction[/]"));

        if (inventory.Entries.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]Your inventory is empty![/]");
            Console.ReadKey(true);
            return;
        }

        // Map inventory entries to displayable item options
        var itemsForSale = inventory.Entries.Select(entry =>
        {
            var item = _itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();
            return item == null ? null : new { item, entry.Count };
        }).Where(x => x != null).ToList();

        // Prompt user to choose which item to list
        var choices = itemsForSale.Select(x => $"{x!.item.ItemName} (x{x.Count})").ToList();
        choices.Add("[red]Go Back[/]");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Choose an item to put on auction[/]")
                .HighlightStyle("cyan")
                .AddChoices(choices));

        if (choice == "[red]Go Back[/]")
            return;

        var selected = itemsForSale.First(x => $"{x!.item.ItemName} (x{x.Count})" == choice);

        // Ask for quantity to list
        int quantityToList;
        if (selected.Count == 1)
        {
            quantityToList = 1;
            AnsiConsole.MarkupLine(
                $"[yellow] You only have 1 [bold]{selected.item.ItemName}[/], listing it automatically.[/]");
        }
        else
        {
            while (true)
            {
                var quantityInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("[green]How many would you like to list? (or type 'back' to cancel)[/]")
                        .PromptStyle("blue")
                        .Validate(input =>
                        {
                            if (input.ToLower() == "back") return ValidationResult.Success();
                            return int.TryParse(input, out var val) && val > 0 && val <= selected.Count
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Invalid quantity.[/]");
                        }));

                if (quantityInput.ToLower() == "back") return;

                quantityToList = int.Parse(quantityInput);
                break;
            }
        }

        // Ask for price per item
        int price;
        while (true)
        {
            var priceInput = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Enter the [bold]price per item[/] (or type 'back' to cancel):[/]")
                    .PromptStyle("blue")
                    .Validate(input =>
                    {
                        if (input.ToLower() == "back") return ValidationResult.Success();
                        return int.TryParse(input, out var val) && val > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Invalid price.[/]");
                    }));

            if (priceInput.ToLower() == "back") return;

            price = int.Parse(priceInput);
            break;
        }

        // Create and insert new auction item
        var auctionItem = new AuctionItem
        {
            ItemId = selected.item.Id!,
            SellerGameId = currentGame.Id.ToString(),
            Price = price,
            Quantity = quantityToList,
            IsSold = false,
            CreatedAt = DateTime.UtcNow
        };

        // Remove listed items from player's inventory
        for (var i = 0; i < quantityToList; i++)
            inventory.RemoveItem(selected.item.Id!);

        _auctionCollection.InsertOne(auctionItem);

        AnsiConsole.Write(
            new Panel(
                    $"[green]{selected.item.ItemName} x{quantityToList}[/] listed for [yellow]${price}[/] each.")
                .Header("[green]Success[/]")
                .Border(BoxBorder.Double)
                .Padding(1, 0, 1, 0));

        Console.ReadKey(true);
    }

    /// <summary>
    /// Allows a player to purchase an auction item.
    /// </summary>
    /// <returns>True if the purchase was successful; otherwise, false.</returns>
    public bool BuyItem(Game buyerGame, InventoryService inventory, BankingService banking, AuctionItem auctionItem,
        int quantityToBuy)
    {
        AnsiConsole.Write(new Rule("[bold blue]Processing Purchase[/]"));

        // Fetch item details
        var item = _itemCollection.Find(i => i.Id == auctionItem.ItemId).FirstOrDefault();
        if (item == null)
        {
            AnsiConsole.MarkupLine("[red]Item not found.[/]");
            return false;
        }

        if (quantityToBuy <= 0 || quantityToBuy > auctionItem.Quantity)
        {
            AnsiConsole.MarkupLine("[red]Invalid quantity selected.[/]");
            return false;
        }

        var totalCost = auctionItem.Price * quantityToBuy;

        if (totalCost > banking._money)
        {
            AnsiConsole.MarkupLine("[bold red][!] Not enough money to complete the purchase![/]");
            return false;
        }

        // Retrieve seller's game object
        var sellerGame = _mongoService.GetGameById(auctionItem.SellerGameId);
        if (sellerGame == null)
        {
            AnsiConsole.MarkupLine("[red]Seller not found.[/]");
            return false;
        }

        // Perform transaction
        banking.RemoveMoney(totalCost);
        sellerGame.Money += totalCost;
        inventory.AddItem(item.Id!, quantityToBuy);

        // Update auction item state
        auctionItem.Quantity -= quantityToBuy;
        if (auctionItem.Quantity == 0)
            auctionItem.IsSold = true;

        auctionItem.BuyerGameId = buyerGame.Id.ToString();

        // Save changes to database
        _auctionCollection.ReplaceOne(x => x.Id == auctionItem.Id, auctionItem);
        _mongoService.SaveGame(sellerGame);

        AnsiConsole.Write(
            new Panel(
                    $"[green]You purchased[/] [bold]{item.ItemName} x{quantityToBuy}[/] [green]for[/] [yellow]{totalCost:C0}[/]")
                .Header("[green]Success[/]")
                .Border(BoxBorder.Ascii));

        return true;
    }

    /// <summary>
    /// Retrieves a specific item by ID from the item collection.
    /// </summary>
    public Item? GetItemById(string itemId)
    {
        return _itemCollection.Find(i => i.Id == itemId).FirstOrDefault();
    }

    /// <summary>
    /// Retrieves a game by its ID using the MongoDbService.
    /// </summary>
    public Game? GetGameById(string id)
    {
        return _mongoService.GetGameById(id);
    }
}
