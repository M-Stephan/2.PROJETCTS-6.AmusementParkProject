using Solution.Data;
using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

public class ShopViews
{
    public void ShopMenu(InventoryService inventoryService, BankingService money)
    {
        var mongoService = new MongoDbService();
        var itemCollection = mongoService.GetItemCollection();
        ShopService shopService = new(itemCollection);

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold underline green]🛒 Shop Menu[/]")
                .PageSize(5)
                .HighlightStyle("aqua")
                .AddChoices("Buy attraction", "Sell Attraction", "Back to main menu")
        );


        switch (choice)
        {
            case "Buy attraction":
                shopService.BuyMenu(inventoryService, money);
                break;
            case "Sell Attraction":
                shopService.SellMenu(inventoryService, money);
                break;
            case "Back to main menu":
                break;
        }
    }
}