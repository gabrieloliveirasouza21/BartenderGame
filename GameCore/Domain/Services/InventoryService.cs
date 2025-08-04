using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<string, int> _stock;
        private readonly Dictionary<string, List<string>> _ingredientTags;

        public InventoryService(Dictionary<string, int> initialStock)
        {
            _stock = initialStock;
            _ingredientTags = InitializeIngredientTags();
        }

        public List<Ingredient> GetAvailableIngredients()
        {
            return _stock.Where(kvp => kvp.Value > 0)
                         .Select(kvp => new Ingredient(kvp.Key, GetTagsFor(kvp.Key)))
                         .ToList();
        }

        public void ConsumeIngredients(List<Ingredient> usedIngredients)
        {
            foreach (var ingredient in usedIngredients)
            {
                if (_stock.ContainsKey(ingredient.Name))
                {
                    _stock[ingredient.Name]--;
                }
            }
        }

        public bool HasIngredients(List<Ingredient> needed)
        {
            return needed.All(i => _stock.ContainsKey(i.Name) && _stock[i.Name] > 0);
        }

        public int GetIngredientStock(string ingredientName)
        {
            return _stock.TryGetValue(ingredientName, out int stock) ? stock : -1;
        }

        public void AddNewIngredient(string name, List<string> tags, int quantity)
        {
            _stock[name] = quantity;
            _ingredientTags[name] = tags;
        }

        public void RestockIngredient(string name, int quantity)
        {
            if (_stock.ContainsKey(name))
            {
                _stock[name] += quantity;
            }
        }

        private List<string> GetTagsFor(string ingredientName)
        {
            return _ingredientTags.TryGetValue(ingredientName, out var tags) ? tags : new List<string>();
        }

        private Dictionary<string, List<string>> InitializeIngredientTags()
        {
            return new Dictionary<string, List<string>>
            {
                { "Café", new List<string> { "Amargo" } },
                { "Leite", new List<string> { "Doce" } },
                { "Canela", new List<string> { "Doce" } },
                { "Açúcar", new List<string>() },
                { "Chocolate", new List<string>() },
                { "Água", new List<string>() }
            };
        }
    }
}
