namespace Bartender.GameCore.Domain.Models
{
    public class RecipeBook
    {
        private readonly List<Recipe> _recipes;

        public RecipeBook()
        {
            _recipes = new List<Recipe> {
            new Recipe("Latte Canela", new List<string> { "Café", "Leite", "Canela" }, "Reconfortante"),
            new Recipe("Espresso", new List<string> { "Café" }, "Energizante")
            };
        }

        public Recipe FindMatchingRecipe(List<Ingredient> ingredients)
        {
            var inputNames = ingredients.Select(i => i.Name).OrderBy(n => n).ToList();
            foreach (var recipe in _recipes)
            {
                var recipeNames = recipe.RequiredIngredients.OrderBy(n => n).ToList();
                if (inputNames.SequenceEqual(recipeNames))
                {
                    return recipe;
                }
            }
            return null;
        }
    }
}
