using Spectre.Console;
using System;
using System.Collections.Generic;

namespace Park
{
    public class InventoryViews
    {
        public void DisplayInventory(List<Item> items)
        {
            var table = new Table();
            table.AddColumn("Icon");
            table.AddColumn("Name");
            table.AddColumn("Amount");

            foreach (var item in items)
            {
                table.AddRow(item._itemIcon, item._itemName, item._itemCount.ToString());
            }

            table.ShowRowSeparators();
            table.Border(TableBorder.Rounded);
            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[grey]Press any key to return at the main menu[/]");
            Console.ReadKey(true);
        }
    }
}
