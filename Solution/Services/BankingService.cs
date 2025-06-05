using Solution.Interfaces;
using Spectre.Console;

namespace Solution.Services;

public class BankingService : IBanking
{
    public BankingService()
    {
        _money = 25000;
    }

    public int _money { get; set; }

    public void RemoveMoney(int amount)
    {
        _money -= amount;
    }

    public int Balance { get; }

    public void AddMoney(int amount)
    {
        _money += amount;
    }

    public void ShowAmount()
    {
        AnsiConsole.MarkupLine($"\n[bold green]💰 Your account balance:[/] [yellow]${_money}[/]\n");
        // AnsiConsole.MarkupLine("[grey italic]Press any key to return to the main menu...[/]");
        Console.ReadKey(true);
    }
}