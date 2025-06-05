using Solution.Models;
using Solution.Services;

namespace Solution.Interfaces;

public interface IAuction
{
    List<AuctionItem> GetAllActiveItems();
    List<AuctionItem> GetAvailableItemsToBuy(string buyerName);
    void ListItem(string sellerName, InventoryService inventoryService);
    bool BuyItem(string buyerName, InventoryService inventoryService, BankingService buyerMoney, AuctionItem auctionItem, int quantityToBuy);
    Item? GetItemById(string itemId);
}