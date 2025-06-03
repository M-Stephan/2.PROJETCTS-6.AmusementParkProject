using System;
using System.Collections.Generic;
using System.Linq;

namespace Park
{
    public interface IInventory
    {
        void AddItem(Item item);
        void RemoveItem(Item item);
    }

    public class Inventory : IInventory
    {
        public List<Item> Items { get; private set; }

        public Inventory()
        {
            Items = new();
        }

        public void AddItem(Item newItem)
        {
            var existingItem = Items.FirstOrDefault(i => i._itemName == newItem._itemName);

            if (existingItem != null)
            {
                existingItem._itemCount += newItem._itemCount;
            }
            else
            {
                Items.Add(newItem);
            }
        }

        public void RemoveItem(Item item)
        {
            var existingItem = Items.FirstOrDefault(i => i._itemName == item._itemName);

            if (existingItem != null)
            {
                existingItem._itemCount--;

                if (existingItem._itemCount <= 0)
                {
                    Items.Remove(existingItem);
                }
            }
            else
            {
                Console.WriteLine("You don't have this attraction in your inventory!");
            }
        }


        public void ShowInventory()
        {
            InventoryViews inventoryViews = new();
            inventoryViews.DisplayInventory(Items);
        }
    }
}
