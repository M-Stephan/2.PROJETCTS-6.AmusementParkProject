using Solution.Models;
using Solution.Services;
using Spectre.Console;

namespace Park.AuctionSystem;

/// <summary>
/// Manages user interactions for the auction system UI.
/// </summary>
public class AuctionUI
{
    private readonly AuctionService _auctionService;

    public AuctionUI(AuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    /// <summary>
    /// Displays the auction menu for the current player.
    /// Allows listing items for auction and purchasing listed items.
    /// </summary>
    public void AuctionMenu(Game currentGame, InventoryService inventory, BankingService money)
    {
        while (true)
        {
            AnsiConsole.Clear();

            // Display auction header panel
            AnsiConsole.Write(
                new Panel("[bold yellow]🏦 Welcome to the Auction House![/]")
                    .Border(BoxBorder.Rounded)
                    .Padding(1, 1, 1, 1)
                    .BorderStyle(new Style(Color.Gold1)));

            // Present menu options
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]📋 Auction Menu[/]")
                    .AddChoices("📤 List Item for Auction", "🛒 View & Buy Auction Items", "⬅️ Back"));

            // Exit the auction menu
            if (choice == "⬅️ Back") break;

            // Handle listing an item for auction
            if (choice == "📤 List Item for Auction")
            {
                _auctionService.ListItem(currentGame, inventory);
            }
            // Handle viewing and buying items from the auction
            else if (choice == "🛒 View & Buy Auction Items")
            {
                // Get all active auction items with quantity > 0
                var allItems = _auctionService.GetAllActiveItems().Where(a => a.Quantity > 0).ToList();
                if (allItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]⚠️ No items currently on auction.[/]");
                    Console.ReadKey(true);
                    continue;
                }

                // Display all auction items with seller info and formatting
                var auctionDisplay = allItems.Select(a =>
                {
                    var item = _auctionService.GetItemById(a.ItemId);
                    var seller = _auctionService.GetGameById(a.SellerGameId);
                    var sellerName = seller?.Name == currentGame.Name
                        ? "[italic grey]You[/]"
                        : $"[italic]{seller?.Name ?? "Unknown"}[/]";
                    return
                        $"[]{item?.ItemIcon ?? "?"}[/] [white]{item?.ItemName ?? "Unknown"}[/] - [green]${a.Price}[/] x[bold]{a.Quantity}[/] (Seller: {sellerName})";
                }).ToList();

                foreach (var line in auctionDisplay)
                    AnsiConsole.MarkupLine("• " + line);
                AnsiConsole.MarkupLine("");

                // Filter out the current user's listings to only show items they can buy
                var buyableItems = allItems.Where(a => a.SellerGameId != currentGame.Id.ToString()).ToList();
                if (buyableItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]🧍 All auction items are yours. Nothing to purchase.[/]");
                    Console.ReadKey(true);
                    continue;
                }

                // Display items available for purchase
                var buyableDisplay = buyableItems.Select(a =>
                {
                    var item = _auctionService.GetItemById(a.ItemId);
                    var seller = _auctionService.GetGameById(a.SellerGameId);
                    return
                        $"[white]{item?.ItemName ?? "Unknown"}[/] - [green]${a.Price}[/] x[bold]{a.Quantity}[/] (Seller: [italic]{seller?.Name ?? "Unknown"}[/])";
                }).ToList();

                // Add option to cancel purchase
                buyableDisplay.Add("[red]❌ Go Back[/]");

                // Prompt user to choose an item to buy
                var choiceItem = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]🛍️ Choose an item to buy[/]")
                        .AddChoices(buyableDisplay));

                if (choiceItem == "[red]❌ Go Back[/]") continue;

                // Identify selected auction item
                var index = buyableDisplay.IndexOf(choiceItem);
                var auctionItem = buyableItems[index];

                var quantityToBuy = 1;

                // Prompt quantity if more than one item available
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

                        if (qtyInput.ToLower() == "back") break;

                        quantityToBuy = int.Parse(qtyInput);
                        break;
                    }

                    if (quantityToBuy == 0) continue;
                }

                // Attempt to purchase the selected item
                var success = _auctionService.BuyItem(currentGame, inventory, money, auctionItem, quantityToBuy);
                if (!success)
                    AnsiConsole.MarkupLine("\n[red]❌ Transaction failed.[/]");

                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }
    }
}
