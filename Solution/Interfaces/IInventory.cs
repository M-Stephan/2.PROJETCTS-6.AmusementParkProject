using MongoDB.Driver;
using Solution.Models;

namespace Solution.Interfaces;

public interface IInventory
{
    void AddItem(string itemId, int count = 1);
    void RemoveItem(string itemId);
    void ShowInventory(IMongoCollection<Item> itemCollection );
    List<InventoryEntry> ToInventoryEntries();
    void LoadFromEntries(List<InventoryEntry> entries);
}