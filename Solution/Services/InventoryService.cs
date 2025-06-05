using MongoDB.Driver;
using Solution.Interfaces;
using Solution.Models;
using Solution.Views;

namespace Solution.Services;

public class InventoryService : IInventory
{
    public List<InventoryEntry> Entries { get; private set; } = new();

    // 🧠 Called by game logic to add an item by ID
    public void AddItem(string itemId, int count = 1)
    {
        var entry = Entries.FirstOrDefault(e => e.ItemId == itemId);
        if (entry != null)
            entry.Count += count;
        else
            Entries.Add(new InventoryEntry
            {
                ItemId = itemId,
                Count = count
            });
    }


    // 🧠 Called by game logic to remove an item by ID
    public void RemoveItem(string itemId)
    {
        var entry = Entries.FirstOrDefault(e => e.ItemId == itemId);
        if (entry != null)
        {
            entry.Count--;
            if (entry.Count <= 0) Entries.Remove(entry);
        }
        else
        {
            Console.WriteLine("You don't have this item in your inventory.");
        }
    }

    // 🔁 Convert inventory to save format
    public List<InventoryEntry> ToInventoryEntries()
    {
        return Entries.Select(e => new InventoryEntry
        {
            ItemId = e.ItemId,
            Count = e.Count
        }).ToList();
    }

    // ⬇ Load from DB entries
    public void LoadFromEntries(List<InventoryEntry> loadedEntries)
    {
        Entries = loadedEntries ?? new List<InventoryEntry>();
    }

    // 👁️ Display inventory with resolved item data from Mongo
    public void ShowInventory(IMongoCollection<Item> itemCollection)
    {
        var detailedItems = new List<Item>();

        foreach (var entry in Entries)
        {
            var dbItem = itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();
            if (dbItem != null)
                // ✅ Use full item data now (includes description & popularity)
                detailedItems.Add(new Item(
                    dbItem.ItemName,
                    dbItem.ItemIcon,
                    entry.Count,
                    dbItem.ItemCost,
                    dbItem.ItemDescription,
                    dbItem.Popularity
                ));
        }

        InventoryViews inventoryViews = new(itemCollection);
        inventoryViews.DisplayInventory(Entries); // You may pass detailedItems too if you want description shown
    }
}