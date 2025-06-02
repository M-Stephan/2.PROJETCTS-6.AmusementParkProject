using System.Collections.Generic;
using InventoryView;

namespace Park
{
    public class Inventory
    {
        public List<Attraction> _attractions { get; set; }

        public Inventory()
        {
            _attractions = new();
        }

        public void AddAttraction(string attraction)
        {
            _attractions.Add(attraction);
        }

        public void RemoveAttraction(string attraction)
        {
            _attractions.Remove(attraction);
        }

        public void ShowInventory(Inventory inventory)
        {
            InventoryView.DisplayInventory(Inventory inventory);
        }
    }
}
