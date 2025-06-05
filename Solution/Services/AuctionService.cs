using MongoDB.Driver;
using Solution.Data;
using Solution.Interfaces;
using Solution.Models;
using Spectre.Console;

namespace Solution.Services;

public class AuctionService : IAuction
{
    private readonly IMongoCollection<AuctionItem> _auctionCollection;
    private readonly IMongoCollection<Item> _itemCollection;
    private readonly MongoDbService _mongoService;

    public AuctionService(IMongoCollection<AuctionItem> auctionCollection, IMongoCollection<Item> itemCollection,
        MongoDbService mongoService)
    {
        _auctionCollection = auctionCollection;
        _itemCollection = itemCollection;
        _mongoService = mongoService;
    }

    // Returns all active (not sold) auction items regardless of seller
    public List<AuctionItem> GetAllActiveItems()
    {
        return _auctionCollection.Find(x => !x.IsSold).ToList();
    }

    // Returns active auction items excluding those sold by the buyer (for buying)
    public List<AuctionItem> GetAvailableItemsToBuy(string buyerName)
    {
        return _auctionCollection.Find(x => !x.IsSold && x.SellerName != buyerName).ToList();
    }

    public void ListItem(string sellerName, InventoryService inventoryService)
    {
        AnsiConsole.Write(new Rule("[bold blue]List Item on Auction[/]"));

        if (inventoryService.Entries.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red][!] Your inventory is empty![/]");
            Console.ReadKey(true);
            return;
        }

        var itemsForSale = inventoryService.Entries.Select(entry =>
        {
            var item = _itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();
            return item == null ? null : new { item, entry.Count };
        }).Where(x => x != null).ToList();

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

                if (quantityInput.ToLower() == "back")
                    return;

                quantityToList = int.Parse(quantityInput);
                break;
            }
        }

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
                            : ValidationResult.Error("[red]Invalid price. Please enter a positive whole number.[/]");
                    }));

            if (priceInput.ToLower() == "back")
                return;

            price = int.Parse(priceInput);
            break;
        }

        var auctionItem = new AuctionItem
        {
            ItemId = selected.item.Id!,
            SellerName = sellerName,
            Price = price,
            Quantity = quantityToList,
            BuyerName = null,
            IsSold = false,
            CreatedAt = DateTime.UtcNow
        };

        for (var i = 0; i < quantityToList; i++)
            inventoryService.RemoveItem(selected.item.Id!);

        _auctionCollection.InsertOne(auctionItem);

        AnsiConsole.Write(
            new Panel(
                    $"[green]:moneybag: {selected.item.ItemName} x{quantityToList}[/] has been listed for [yellow]${price}[/] each.")
                .Border(BoxBorder.Double)
                .Header("[green]Success[/]")
                .Padding(1, 0, 1, 0));

        Console.ReadKey(true);
    }

    public bool BuyItem(string buyerName, InventoryService inventoryService, BankingService buyerMoney, AuctionItem auctionItem,
        int quantityToBuy)
    {
        AnsiConsole.Write(new Rule("[bold blue]Processing Purchase[/]"));

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

        if (totalCost > buyerMoney._money)
        {
            AnsiConsole.MarkupLine("[bold red][!] Not enough money to complete the purchase![/]");
            return false;
        }

        var sellerGame = _mongoService.GetGameByNickname(auctionItem.SellerName);
        if (sellerGame == null)
        {
            AnsiConsole.MarkupLine("[red]Seller game not found.[/]");
            return false;
        }

        buyerMoney.RemoveMoney(totalCost);
        sellerGame.Money += totalCost;
        inventoryService.AddItem(auctionItem.ItemId, quantityToBuy);

        if (auctionItem.Quantity == quantityToBuy)
            auctionItem.IsSold = true;

        auctionItem.Quantity -= quantityToBuy;
        auctionItem.BuyerName = buyerName;

        var filter = Builders<AuctionItem>.Filter.Eq(x => x.Id, auctionItem.Id);
        _auctionCollection.ReplaceOne(filter, auctionItem);

        _mongoService.SaveGame(sellerGame);

        AnsiConsole.Write(new Panel(
                $"[green]You purchased [bold]{item.ItemName} x{quantityToBuy}[/] for [yellow]${totalCost}[/]![/]")
            .Header("[green]Success[/]")
            .Border(BoxBorder.Ascii));

        return true;
    }


    public Item? GetItemById(string itemId)
    {
        return _itemCollection.Find(i => i.Id == itemId).FirstOrDefault();
    }
}