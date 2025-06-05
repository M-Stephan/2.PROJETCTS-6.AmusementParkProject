using Spectre.Console;

namespace Solution.Services;

public class MoneyGameService
{
    private readonly BankingService _bankingService;

    public MoneyGameService(BankingService bankingService)
    {
        _bankingService = bankingService;
    }

    public void Play()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("🎰 Lucky Spin!")
                .Color(Color.Green));

        var random = new Random();
        var secretNumber = random.Next(1, 6); // Random number from 1 to 5

        var userGuess = AnsiConsole.Prompt(
            new TextPrompt<int>("[bold cyan]🎯 Guess a number between [underline]1[/] and [underline]5[/]:[/]")
                .PromptStyle("bold yellow")
                .ValidationErrorMessage("[red]⛔ Please enter a valid number between 1 and 5.[/]")
                .Validate(num => num >= 1 && num <= 5));

        if (userGuess == secretNumber)
        {
            var reward = 5000;
            _bankingService._money += reward;

            AnsiConsole.MarkupLine("\n[bold green]✅ Correct![/]");
            AnsiConsole.MarkupLine($"[bold yellow]💰 You won [underline]{reward}[/] coins![/]");
            AnsiConsole.Status()
                .Start("Updating bank account...", ctx =>
                {
                    Thread.Sleep(1000); // Simulate processing
                });
        }
        else
        {
            AnsiConsole.MarkupLine("\n[bold red]❌ Wrong guess![/]");
            AnsiConsole.MarkupLine($"[yellow]The correct number was [underline]{secretNumber}[/].[/]");
        }

        AnsiConsole.MarkupLine("\n[grey]🎮 Press any key to return to the menu...[/]");
        Console.ReadKey(true);
    }
}