using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

public class AuctionUI
{
    private readonly AuctionService _auctionService;

    public AuctionUI(AuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    public void AuctionMenu(string playerName, InventoryService inventoryService, BankingService money)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("[bold yellow]🏦 Welcome to the Auction House![/]")
                    .Border(BoxBorder.Rounded)
                    .Padding(1, 1, 1, 1)
                    .BorderStyle(new Style(Color.Gold1)));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]📋 Auction Menu[/]")
                    .AddChoices("📤 List Item for Auction", "🛒 View & Buy Auction Items", "⬅️ Back"));

            if (choice == "⬅️ Back")
                break;

            if (choice == "📤 List Item for Auction")
            {
                _auctionService.ListItem(playerName, inventoryService);
            }
            else if (choice == "🛒 View & Buy Auction Items")
            {
                var allItems = _auctionService.GetAllActiveItems()
                    .Where(a => a.Quantity > 0)
                    .ToList();

                if (allItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]⚠️ No items currently on auction.[/]");
                    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    continue;
                }

                AnsiConsole.MarkupLine("\n[blue bold]🧾 Auction Items:[/]\n");

                var auctionDisplay = allItems.Select(a =>
                {
                    var item = _auctionService.GetItemById(a.ItemId);
                    var sellerLabel = a.SellerName == playerName ? "[italic grey]You[/]" : $"[italic]{a.SellerName}[/]";
                    return
                        $"[]{item?.ItemIcon ?? "Unknown"}[/] [white]{item?.ItemName ?? "Unknown"}[/] - [green]${a.Price}[/] x[bold]{a.Quantity}[/] (Seller: {sellerLabel})";
                }).ToList();

                foreach (var line in auctionDisplay)
                    AnsiConsole.MarkupLine("• " + line);
                AnsiConsole.MarkupLine("");

                var buyableItems = allItems.Where(a => a.SellerName != playerName).ToList();

                if (buyableItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]🧍 All auction items are yours. Nothing to purchase.[/]");
                    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    continue;
                }

                var buyableDisplay = buyableItems.Select(a =>
                {
                    var item = _auctionService.GetItemById(a.ItemId);
                    return
                        $"[white]{item?.ItemName ?? "Unknown"}[/] - [green]${a.Price}[/] x[bold]{a.Quantity}[/] (Seller: [italic]{a.SellerName}[/])";
                }).ToList();

                buyableDisplay.Add("[red]❌ Go Back[/]");

                var choiceItem = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]🛍️ Choose an item to buy[/]")
                        .AddChoices(buyableDisplay));

                if (choiceItem == "[red]❌ Go Back[/]")
                    continue;

                var index = buyableDisplay.IndexOf(choiceItem);
                var auctionItem = buyableItems[index];

                var quantityToBuy = 1;

                if (auctionItem.Quantity > 1)
                {
                    while (true)
                    {
                        var qtyInput = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]🔢 Enter quantity to buy (or type 'back' to cancel):[/]")
                                .PromptStyle("bold yellow")
                                .ValidationErrorMessage("[red]❗ Invalid quantity entered.[/]")
                                .Validate(input =>
                                {
                                    if (input.ToLower() == "back") return ValidationResult.Success();
                                    return int.TryParse(input, out var val) && val > 0 && val <= auctionItem.Quantity
                                        ? ValidationResult.Success()
                                        : ValidationResult.Error("[red]Invalid quantity.[/]");
                                }));

                        if (qtyInput.ToLower() == "back")
                            break;

                        quantityToBuy = int.Parse(qtyInput);
                        break;
                    }

                    if (quantityToBuy == 0)
                        continue;
                }

                var success = _auctionService.BuyItem(playerName, inventoryService, money, auctionItem, quantityToBuy);

                if (!success)
                    AnsiConsole.MarkupLine("\n[red]❌ Transaction failed: Not enough quantity or funds.[/]");
                else
                    AnsiConsole.MarkupLine(
                        $"\n[bold green]✅ Success![/] You bought [yellow]{quantityToBuy}[/] item(s) for [green]${auctionItem.Price * quantityToBuy}[/]!");

                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }
    }
}