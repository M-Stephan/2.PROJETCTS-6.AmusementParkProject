using Spectre.Console;

namespace Park
{
    public class Shop
    {

        public Inventory _inventory = new();

        public void BuyMenu(Inventory inventory, Banking money)
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[green]Choose an attraction to buy[/]")
                .AddChoices(
                    "Ferris wheel : $25060",
                    "Roller Coaster : $15170",
                    "Carousel : $7550",
                    "Food Stand : $3250",
                    "Ticket Booth : $7500",
                    "Bumper Cars : $12320",
                    "Water Slide : $13480",
                    "Swing Ride : $12865",
                    "Back to main menu"
                ));

            if (choice == "Back to main menu")
                return; // On quitte la méthode ici

            Item? item = choice switch
            {
                "Ferris wheel : $25060" => new Item("Ferris wheel", "[gold1]🎡[/]", 1, 25060),
                "Roller Coaster : $15170" => new Item("Roller Coaster", "[red]🎢[/]", 1, 15170),
                "Carousel : $7550" => new Item("Carousel", "[orchid1]🎠[/]", 1, 7550),
                "Food Stand : $3250" => new Item("Food Stand", "[yellow1]🌭[/]", 1, 3250),
                "Ticket Booth : $7500" => new Item("Ticket Booth", "[blue]🎫[/]", 1, 7500),
                "Bumper Cars : $12320" => new Item("Bumper Cars", "[orange1]🚗[/]", 1, 12320),
                "Water Slide : $13480" => new Item("Water Slide", "[deepskyblue1]🌊[/]", 1, 13480),
                "Swing Ride : $12865" => new Item("Swing Ride", "[violet]🎑[/]", 1, 12865),
                _ => null
            };


            if (item != null && item._itemCost <= money._money)
            {
                inventory.AddItem(item);
                money.RemoveMoney(item._itemCost);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]You don't have enough money.[/]\n");
                AnsiConsole.MarkupLine("\n[grey]Press any key to return at the main menu[/]");
                Console.ReadKey(true);
            }
        }

        public void SellMenu(Inventory inventory, Banking money)
        {
            if (inventory.Items.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Your inventory is empty![/]\n");
                AnsiConsole.MarkupLine("\n[grey]Press any key to return at the main menu[/]");
                Console.ReadKey(true);
                return;
            }

            var itemName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[green]Choose an attraction to sell[/]")
                .AddChoices(inventory.Items.Select(i => i._itemName)));

            var itemToSell = inventory.Items.FirstOrDefault(i => i._itemName == itemName);

            if (itemToSell != null)
            {
                inventory.RemoveItem(itemToSell);
                money.AddMoney(itemToSell._itemCost);
            }
        }
    }
}