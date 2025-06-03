using Spectre.Console;

namespace Park
{
    public class ShopViews
    {
        

        public void ShopMenu(Inventory inventory, Banking money)
        {
            Shop shop = new();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[green]Shop Menu[/]")
                .AddChoices("Buy attraction", "Sell Attraction", "Back to main menu"));

            switch (choice)
            {
                case "Buy attraction":
                    shop.BuyMenu(inventory, money);
                    break;
                case "Sell Attraction":
                    shop.SellMenu(inventory, money);
                    break;
                case "Back to main menu":
                    break;
            }
        }
    }
}
