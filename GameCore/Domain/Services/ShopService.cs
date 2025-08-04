using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class ShopService : IShopService
    {
        private readonly IInventoryService _inventoryService;
        private readonly List<ShopItem> _shopItems;

        public ShopService(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            _shopItems = InitializeShopItems();
        }

        public List<ShopItem> GetAvailableItems()
        {
            var availableItems = new List<ShopItem>();
            
            // Adiciona novos ingredientes
            availableItems.AddRange(_shopItems.Where(item => item.Type == ShopItemType.NewIngredient));
            
            // Adiciona reposi��o de ingredientes existentes
            foreach (var item in _shopItems.Where(item => item.Type == ShopItemType.IngredientRestock))
            {
                if (_inventoryService.GetIngredientStock(item.Name) >= 0) // Se o ingrediente existe no invent�rio
                {
                    availableItems.Add(item);
                }
            }

            return availableItems;
        }

        public bool CanPurchase(ShopItem item, int playerMoney)
        {
            return playerMoney >= item.Price;
        }

        public void PurchaseItem(ShopItem item)
        {
            if (item.Type == ShopItemType.NewIngredient)
            {
                _inventoryService.AddNewIngredient(item.Name, item.Tags, item.Quantity);
            }
            else if (item.Type == ShopItemType.IngredientRestock)
            {
                _inventoryService.RestockIngredient(item.Name, item.Quantity);
            }
        }

        private List<ShopItem> InitializeShopItems()
        {
            return new List<ShopItem>
            {
                // Novos ingredientes
                new ShopItem("Baunilha", new List<string> { "Doce", "Arom�tico" }, 25, 3, ShopItemType.NewIngredient),
                new ShopItem("Gengibre", new List<string> { "Picante", "Estimulante" }, 30, 3, ShopItemType.NewIngredient),
                new ShopItem("Mel", new List<string> { "Doce", "Natural" }, 20, 4, ShopItemType.NewIngredient),
                new ShopItem("Lim�o", new List<string> { "�cido", "Refrescante" }, 15, 5, ShopItemType.NewIngredient),
                new ShopItem("Hortel�", new List<string> { "Refrescante", "Mentolado" }, 18, 4, ShopItemType.NewIngredient),
                
                // Reposi��o de ingredientes existentes
                new ShopItem("Caf�", new List<string> { "Amargo" }, 10, 5, ShopItemType.IngredientRestock),
                new ShopItem("Leite", new List<string> { "Doce" }, 8, 5, ShopItemType.IngredientRestock),
                new ShopItem("Canela", new List<string> { "Doce" }, 12, 3, ShopItemType.IngredientRestock),
                new ShopItem("A��car", new List<string>(), 5, 5, ShopItemType.IngredientRestock),
                new ShopItem("Chocolate", new List<string>(), 15, 3, ShopItemType.IngredientRestock),
                new ShopItem("�gua", new List<string>(), 2, 10, ShopItemType.IngredientRestock)
            };
        }
    }
}