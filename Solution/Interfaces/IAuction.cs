using Solution.Models;
using Solution.Services;
using System.Collections.Generic;

namespace Solution.Interfaces;

public interface IAuction
{
    List<AuctionItem> GetAllActiveItems();

    List<AuctionItem> GetAvailableItemsToBuy(string currentGameId);

    void ListItem(Game currentGame, InventoryService inventory);

    bool BuyItem(Game buyerGame, InventoryService inventory, BankingService banking, AuctionItem auctionItem, int quantityToBuy);

    Item? GetItemById(string itemId);

    Game? GetGameById(string id);
}