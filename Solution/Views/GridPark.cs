using Spectre.Console;
using System;

namespace Park
{
    public class GridPark
    {
        public void DisplayGrid(string[,] grid)
        {
            var table = new Table();
            table.AddColumn("X/Y");

            for (int i = 1; i <= 10; i++)
                table.AddColumn(i.ToString());

            for (int i = 0; i < 10; i++)
            {
                var row = new string[11];
                row[0] = (i + 1).ToString();

                for (int j = 0; j < 10; j++)
                    row[j + 1] = grid[i, j];

                table.AddRow(row);
            }

            table.Border(TableBorder.Rounded);
            table.ShowRowSeparators();
            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[grey]Press Any key to exit...[/]");
            Console.ReadKey(true);
        }
    }
}
