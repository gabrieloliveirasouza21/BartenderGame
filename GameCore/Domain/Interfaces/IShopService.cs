using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface IShopService
    {
        List<ShopItem> GetAvailableItems();
        bool CanPurchase(ShopItem item, int playerMoney);
        void PurchaseItem(ShopItem item);
    }
}