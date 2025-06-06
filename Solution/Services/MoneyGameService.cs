using Spectre.Console;

namespace Solution.Services;

/// <summary>
/// Mini-game that allows the player to guess a number and win money if correct.
/// </summary>
public class MoneyGame
{
    private readonly BankingService _banking;

    public MoneyGame(BankingService banking)
    {
        _banking = banking;
    }

    /// <summary>
    /// Starts the guessing game. Rewards the player with money on a correct guess.
    /// </summary>
    public void Play()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("🎰 Lucky Spin!")
                .Color(Color.Green));

        var random = new Random();
        var secretNumber = random.Next(1, 6);

        var userGuess = AnsiConsole.Prompt(
            new TextPrompt<int>("[bold cyan]🎯 Guess a number between [underline]1[/] and [underline]5[/]:[/]")
                .PromptStyle("bold yellow")
                .ValidationErrorMessage("[red]⛔ Please enter a valid number between 1 and 5.[/]")
                .Validate(num => num >= 1 && num <= 5));

        if (userGuess == secretNumber)
        {
            var reward = 5000;
            _banking._money += reward;

            AnsiConsole.MarkupLine("\n[bold green]✅ Correct![/]");
            AnsiConsole.MarkupLine($"[bold yellow]💰 You won [underline]{reward}[/] coins![/]");

            AnsiConsole.Status()
                .Start("Updating bank account...", ctx => { Thread.Sleep(1000); });
        }
        else
        {
            AnsiConsole.MarkupLine("\n[bold red]❌ Wrong guess![/]");
            AnsiConsole.MarkupLine($"[yellow]The correct number was [underline]{secretNumber}[/].[/]");
        }

        Console.ReadKey(true);
    }
}