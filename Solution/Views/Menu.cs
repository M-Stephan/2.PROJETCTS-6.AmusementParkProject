using MongoDB.Driver;
using Park;
using Park.AuctionSystem;
using Solution.Models;
using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

public class Menu
{
    private readonly MongoDbService _mongoService;
    private readonly GamePersistenceManager _persistenceManager;
    private MoneyGame? _moneyGame;

    public Menu()
    {
        _mongoService = new MongoDbService();
        _persistenceManager = new GamePersistenceManager();
    }

    public void DisplayMenu()
    {
        var itemCollection = _mongoService.GetItemCollection();
        var auctionCollection = _mongoService.GetAuctionCollection();

        var auctionService = new AuctionService(auctionCollection, itemCollection, _mongoService);
        var auctionUI = new AuctionUI(auctionService);

        // Show fancy banner with gradient and rule
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Fun Park!")
                .Centered()
                .Color(new Color(255, 165, 0))); // Orange

        AnsiConsole.WriteLine();

        AnsiConsole.Render(
            new Rule("[deepskyblue3_1 bold]Welcome to the Amusement Park Simulator![/]")
                .RuleStyle("deepskyblue3_1")
                .Centered());

        AnsiConsole.MarkupLine("[grey]Press any key to start...[/]");
        Console.ReadKey(true);

        // Load or create game
        Game currentGame;
        var allGames = _mongoService.GetAllGames();

        if (allGames.Any())
        {
            var startChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green underline]Do you want to continue a previous game or start a new game?[/]")
                    .AddChoices("Continue", "New Game")
                    .HighlightStyle("green")
                    .PageSize(5));

            if (startChoice == "Continue")
            {
                var names = allGames.Select(g => g.Name).ToList();
                var selectedNickname = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green underline]Select your game by nickname:[/]")
                        .AddChoices(names)
                        .HighlightStyle("green")
                        .PageSize(5));

                var loadedGame = _mongoService.GetGameByNickname(selectedNickname);
                currentGame = loadedGame ?? CreateNewGamePrompt(itemCollection);
            }
            else
            {
                currentGame = CreateNewGamePrompt(itemCollection);
            }
        }
        else
        {
            currentGame = CreateNewGamePrompt(itemCollection);
        }

        // Setup runtime state
        var gridState = new GridStateService(itemCollection);
        gridState.LoadFromListGrid(currentGame.Grid);

        var banking = new BankingService() { _money = currentGame.Money };
        var inventory = new InventoryService();
        _moneyGame = new MoneyGame(banking);

        foreach (var entry in currentGame.Inventory)
            inventory.AddItem(entry.ItemId, entry.Count);

        var gridPark = new GridPark(itemCollection);
        var shopView = new ShopViews();
        var mover = new MoveAttraction(itemCollection, _mongoService, currentGame);

        // Menu options with emojis and colors
        var menuOptions = new List<string>
        {
            "[bold cyan]" + "Display Park Grid".PadRight(10) + "[/]",
            "[bold yellow]" + "View Bank".PadRight(10) + "[/]",
            "[bold magenta]" + "View Inventory".PadRight(10) + "[/]",
            "[bold green]" + "Go to the Shop".PadRight(10) + "[/]",
            "[bold orange1]" + "Place a Ride".PadRight(10) + "[/]",
            "[bold red]" + "Remove a Ride".PadRight(10) + "[/]",
            "[bold purple]" + "Money Gain".PadRight(10) + "[/]",
            "[bold teal]" + "Auction House".PadRight(10) + "[/]",
            "[bold darkred]" + "Delete game".PadRight(10) + "[/]",
            "[bold springgreen3]" + "Save & Exit".PadRight(10) + "[/]"
        };


        // Main menu loop
        while (true)
        {
            AnsiConsole.Clear();

            // Show money and player name at top in a panel
            var headerPanel = new Panel(
                    new Markup(
                        $"[bold yellow]Player:[/] [bold orange1]{currentGame.Name}[/]\n[bold yellow]Money:[/] [bold green]{banking._money:C0}[/]"))
                .BorderColor(Color.Orange1)
                .Header("[bold white on darkblue] 🎡 Amusement Park Simulator 🎡 [/]")
                .Expand();

            AnsiConsole.Render(headerPanel);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]🎠 Choose an option below 🎢[/]")
                    .PageSize(10)
                    .HighlightStyle("aqua")
                    .AddChoices(menuOptions));

            var selectedIndex = menuOptions.IndexOf(selected);


            switch (selectedIndex)
            {
                case 0:
                    gridPark.DisplayGrid(gridState.Grid);
                    break;

                case 1:
                    banking.ShowAmount();
                    break;

                case 2:
                    inventory.ShowInventory(itemCollection);
                    break;

                case 3:
                    shopView.ShopMenu(inventory, banking);
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    break;

                case 4:
                    mover.PlaceItem(inventory, gridState);
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    break;

                case 5:
                    mover.RemoveItem(inventory, gridState);
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    break;

                case 6:
                    AnsiConsole.MarkupLine("[grey]Launching Money Game...[/]");
                    _moneyGame!.Play();
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    break;


                case 7:
                    auctionUI.AuctionMenu(currentGame, inventory, banking);
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    break;


                case 8:
                    _persistenceManager.DeleteGame(currentGame);
                    break;

                case 9:
                    _persistenceManager.SaveGame(currentGame, inventory, banking, gridState);
                    AnsiConsole.MarkupLine("[green bold]Game saved successfully. Goodbye![/]");
                    Environment.Exit(0);
                    break;
            }

            AnsiConsole.MarkupLine("\n[grey italic]Press any key to return to menu...[/]");
            Console.ReadKey(true);
        }
    }

    private Game CreateNewGamePrompt(IMongoCollection<Item> itemCollection)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("New Game")
                .Centered()
                .Color(Color.SpringGreen3));

        string name;

        while (true)
        {
            name = AnsiConsole.Ask<string>("[green]Hi, how should I call you?[/]");

            // Check if a game with this name already exists
            var existingGame = _mongoService.GetGameByNickname(name);
            if (existingGame != null)
            {
                AnsiConsole.MarkupLine("[red]This game already exists. Please choose a different one.[/]");
            }
            else
            {
                break;
            }
        }

        var newGame = new Game
        {
            Name = name,
            Money = 25000,
            Inventory = new List<InventoryEntry>(),
            Grid = new GridStateService(itemCollection).ToListGrid()
        };

        // Save new game to MongoDB immediately
        _mongoService.SaveGame(newGame);

        AnsiConsole.MarkupLine("[green]New game created and saved to database![/]");

        return newGame;
    }
}