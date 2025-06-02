using Spectre.Console;
using System;

namespace Park
{
    public class InventoryView
    {
        public void DisplayInventory(Inventory inventory)
        {
            var table = new Table();
            {
                table.AddColumn("Icon");
                table.AddColumn("Name");
                table.AddColumn("Amount");
                foreach (var item in inventory)
                {
                    table.AddRow(item._itemName, item._itemIcon, item._itemCount);
                }
                table.ShowRowSeparators();
                AnsiConsole.Write(table);
            }


        }
    }
}
