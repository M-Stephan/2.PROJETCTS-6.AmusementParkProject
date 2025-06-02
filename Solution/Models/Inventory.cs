using System.Collections.Generic;

namespace Park
{
    public interface IInventory
    {
        public void AddItem(Item item);
    }

    public class Inventory : IInventory
    {
        public List<Item> _items { get; private set; }

        public Inventory()
        {
            _items = new();
        }

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public void RemoveAttraction(Item item)
        {
            _items.Remove(item);
        }

        public void ShowInventory()
        {
            InventoryViews inventoryViews = new();
            inventoryViews.DisplayInventory(_items);
        }

        public void LoadDefaultItems()
        {
            _items.AddRange(new List<Item>
            {
                new Item("Ferris wheel", "[gold1]🎡[/]", 3),
                new Item("Roller Coaster", "[red]🎢[/]", 6),
                new Item("Carousel", "[orchid1]🎠[/]", 2),
                new Item("Food Stand", "[yellow1]🌭[/]", 4),
                new Item("Ticket Booth", "[blue]🎫[/]", 1),
                new Item("Bumper Cars", "[orange1]🚗[/]", 2),
                new Item("Water Slide", "[deepskyblue1]🌊[/]", 2),
                new Item("Swing Ride", "[violet]🎑[/]", 1)
            });
        }
    }
}
