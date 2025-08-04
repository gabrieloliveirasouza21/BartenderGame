using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class ShopService : IShopService
    {
        private readonly IInventoryService _inventoryService;
        private readonly List<ShopItem> _shopItems;
        private readonly HashSet<string> _purchasedNewIngredients;
        private readonly Random _random;
        private const int MAX_SHOP_ITEMS = 3;

        public ShopService(IInventoryService inventoryService) : this(inventoryService, null)
        {
        }

        // Construtor para testes com seed determinística
        public ShopService(IInventoryService inventoryService, int? randomSeed)
        {
            _inventoryService = inventoryService;
            _shopItems = InitializeShopItems();
            _purchasedNewIngredients = new HashSet<string>();
            _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
        }

        public List<ShopItem> GetAvailableItems()
        {
            var allPossibleItems = new List<ShopItem>();
            
            // Adiciona novos ingredientes que ainda não foram comprados
            var newIngredientsAvailable = _shopItems
                .Where(item => item.Type == ShopItemType.NewIngredient && !_purchasedNewIngredients.Contains(item.Name));
            allPossibleItems.AddRange(newIngredientsAvailable);
            
            // Adiciona reposição de ingredientes existentes (originais do jogo)
            foreach (var item in _shopItems.Where(item => item.Type == ShopItemType.IngredientRestock))
            {
                if (_inventoryService.GetIngredientStock(item.Name) >= 0) // Se o ingrediente existe no inventário
                {
                    allPossibleItems.Add(item);
                }
            }

            // Adiciona reposição de ingredientes que foram comprados anteriormente como novos
            foreach (var purchasedIngredientName in _purchasedNewIngredients)
            {
                if (_inventoryService.GetIngredientStock(purchasedIngredientName) >= 0) // Se existe no inventário
                {
                    var originalItem = _shopItems.First(item => item.Name == purchasedIngredientName && item.Type == ShopItemType.NewIngredient);
                    // Cria um item de reposição baseado no item original, mas com preço e quantidade reduzidos
                    var restockItem = new ShopItem(
                        originalItem.Name, 
                        originalItem.Tags, 
                        (int)(originalItem.Price * 0.6), // 60% do preço original para reposição
                        Math.Max(1, originalItem.Quantity - 1), // Quantidade um pouco menor
                        ShopItemType.IngredientRestock);
                    
                    allPossibleItems.Add(restockItem);
                }
            }

            // Seleciona aleatoriamente até 3 itens da lista de possíveis itens
            return SelectRandomItems(allPossibleItems, MAX_SHOP_ITEMS);
        }

        // Método para testes que retorna todos os itens possíveis
        public List<ShopItem> GetAllPossibleItems()
        {
            var allPossibleItems = new List<ShopItem>();
            
            // Adiciona novos ingredientes que ainda não foram comprados
            var newIngredientsAvailable = _shopItems
                .Where(item => item.Type == ShopItemType.NewIngredient && !_purchasedNewIngredients.Contains(item.Name));
            allPossibleItems.AddRange(newIngredientsAvailable);
            
            // Adiciona reposição de ingredientes existentes (originais do jogo)
            foreach (var item in _shopItems.Where(item => item.Type == ShopItemType.IngredientRestock))
            {
                if (_inventoryService.GetIngredientStock(item.Name) >= 0)
                {
                    allPossibleItems.Add(item);
                }
            }

            // Adiciona reposição de ingredientes que foram comprados anteriormente como novos
            foreach (var purchasedIngredientName in _purchasedNewIngredients)
            {
                if (_inventoryService.GetIngredientStock(purchasedIngredientName) >= 0)
                {
                    var originalItem = _shopItems.First(item => item.Name == purchasedIngredientName && item.Type == ShopItemType.NewIngredient);
                    var restockItem = new ShopItem(
                        originalItem.Name, 
                        originalItem.Tags, 
                        (int)(originalItem.Price * 0.6),
                        Math.Max(1, originalItem.Quantity - 1),
                        ShopItemType.IngredientRestock);
                    
                    allPossibleItems.Add(restockItem);
                }
            }

            return allPossibleItems;
        }

        private List<ShopItem> SelectRandomItems(List<ShopItem> allItems, int maxItems)
        {
            if (allItems.Count <= maxItems)
            {
                // Se temos 3 ou menos itens, retorna todos
                return allItems.ToList();
            }

            // Embaralha a lista e pega os primeiros 'maxItems' itens
            var shuffledItems = allItems.OrderBy(x => _random.Next()).ToList();
            return shuffledItems.Take(maxItems).ToList();
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
                // Marca o novo ingrediente como comprado para que não apareça mais na loja
                _purchasedNewIngredients.Add(item.Name);
            }
            else if (item.Type == ShopItemType.IngredientRestock)
            {
                _inventoryService.RestockIngredient(item.Name, item.Quantity);
                // Reposição pode ser comprada várias vezes, então não adicionamos à lista de comprados
            }
        }

        private List<ShopItem> InitializeShopItems()
        {
            return new List<ShopItem>
            {
                // Novos ingredientes
                new ShopItem("Baunilha", new List<string> { "Doce", "Aromático" }, 25, 3, ShopItemType.NewIngredient),
                new ShopItem("Gengibre", new List<string> { "Picante", "Estimulante" }, 30, 3, ShopItemType.NewIngredient),
                new ShopItem("Mel", new List<string> { "Doce", "Natural" }, 20, 4, ShopItemType.NewIngredient),
                new ShopItem("Limão", new List<string> { "Ácido", "Refrescante" }, 15, 5, ShopItemType.NewIngredient),
                new ShopItem("Hortelã", new List<string> { "Refrescante", "Mentolado" }, 18, 4, ShopItemType.NewIngredient),
                
                // Reposição de ingredientes existentes
                new ShopItem("Café", new List<string> { "Amargo" }, 10, 5, ShopItemType.IngredientRestock),
                new ShopItem("Leite", new List<string> { "Doce" }, 8, 5, ShopItemType.IngredientRestock),
                new ShopItem("Canela", new List<string> { "Doce" }, 12, 3, ShopItemType.IngredientRestock),
                new ShopItem("Açúcar", new List<string>(), 5, 5, ShopItemType.IngredientRestock),
                new ShopItem("Chocolate", new List<string>(), 15, 3, ShopItemType.IngredientRestock),
                new ShopItem("Água", new List<string>(), 2, 10, ShopItemType.IngredientRestock)
            };
        }
    }
}