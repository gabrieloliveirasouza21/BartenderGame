using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<string, int> _stock;

        public InventoryService(Dictionary<string, int> initialStock)
        {
            _stock = initialStock;
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

        private List<string> GetTagsFor(string ingredientName)
        {
            return ingredientName switch
            {
                "Café" => new List<string> { "Amargo" },
                "Leite" => new List<string> { "Doce" },
                "Canela" => new List<string> { "Doce" },
                _ => new List<string>()
            };
        }
    }
}
