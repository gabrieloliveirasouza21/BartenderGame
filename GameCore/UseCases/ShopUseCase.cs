using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.UseCases
{
    public class ShopUseCase
    {
        private readonly IShopService _shopService;
        private readonly EventBus _eventBus;

        public ShopUseCase(IShopService shopService, EventBus eventBus)
        {
            _shopService = shopService;
            _eventBus = eventBus;
        }

        public virtual List<ShopItem> GetAvailableItems()
        {
            return _shopService.GetAvailableItems();
        }

        public virtual bool CanPurchase(ShopItem item, int playerMoney)
        {
            return _shopService.CanPurchase(item, playerMoney);
        }

        public virtual bool PurchaseItem(ShopItem item, GameState gameState)
        {
            if (!_shopService.CanPurchase(item, gameState.Money))
            {
                return false;
            }

            _shopService.PurchaseItem(item);
            gameState.SpendMoney(item.Price);
            
            _eventBus.Publish(new ItemPurchasedEvent(item, gameState.Money));
            
            return true;
        }
    }
}