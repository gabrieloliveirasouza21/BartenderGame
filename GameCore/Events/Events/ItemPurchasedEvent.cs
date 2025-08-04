using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class ItemPurchasedEvent : IEvent
    {
        public ShopItem Item { get; }
        public int RemainingMoney { get; }

        public ItemPurchasedEvent(ShopItem item, int remainingMoney)
        {
            Item = item;
            RemainingMoney = remainingMoney;
        }
    }
}