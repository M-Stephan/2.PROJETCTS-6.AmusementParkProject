using Spectre.Console;

namespace Park
{
    public class Banking
    {
        public int _money { get; set; }

        public Banking()
        {
            _money = 25000; // Default money in the Bank
        }

        public void RemoveMoney(int amount)
        {
            _money -= amount;
        }

        public void AddMoney(int amount)
        {
            _money += amount;
        }

        public void ShowAmount()
        {
            AnsiConsole.MarkupLine("Your account has $" + _money + " amount.");
            AnsiConsole.MarkupLine("\n[grey]Press any key to return at the main menu[/]");
            Console.ReadKey(true);
        }
    }
} 