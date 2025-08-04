using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class CraftService : ICraftService
    {
        private readonly RecipeBook _recipeBook;

        public CraftService(RecipeBook recipeBook)
        {
            _recipeBook = recipeBook;
        }

        public Drink Craft(List<Ingredient> ingredients)
        {
            var recipe = _recipeBook.FindMatchingRecipe(ingredients);
            if (recipe != null)
            {
                return new Drink(recipe.Name, ingredients, recipe.Effect);
            }
            else
            {
                return CraftDynamicDrink(ingredients);
            }
        }

        private Drink CraftDynamicDrink(List<Ingredient> ingredients)
        {
            var allTags = ingredients.SelectMany(i => i.Tags);
            var drinkName = $"Improvisado: {string.Join(", ", ingredients.Select(i => i.Name))}";
            
            if (!allTags.Any())
            {
                // Se não há tags, criar drink neutro
                return new Drink(drinkName, ingredients, "Neutro");
            }
            
            var dominantTag = allTags
                .GroupBy(tag => tag)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var effect = TagToEffect(dominantTag);
            return new Drink(drinkName, ingredients, effect);
        }

        private string TagToEffect(string tag) => tag switch
        {
            "Doce" => "Reconfortante",
            "Amargo" => "Energizante",
            "Refrescante" => "Revigorante",
            _ => "Neutro"
        };
    }
}
