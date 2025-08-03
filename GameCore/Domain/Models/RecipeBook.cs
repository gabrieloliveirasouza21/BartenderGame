namespace Bartender.GameCore.Domain.Models
{
    public class RecipeBook
    {
        private readonly List<Recipe> _recipes;

        public RecipeBook()
        {
            _recipes = new List<Recipe> {
            new Recipe("Latte Canela", new List<string> { "Café", "Leite", "Canela" }, "Reconfortante"),
            new Recipe("Espresso", new List<string> { "Café" }, "Energizante"),
            new Recipe("Café Doce", new List<string> { "Café", "Açúcar" }, "Reconfortante"),
            new Recipe("Chocolate Quente", new List<string> { "Chocolate", "Leite" }, "Reconfortante"),
            new Recipe("Água", new List<string> { "Água" }, "Neutro")
            };
        }

        public Recipe? FindMatchingRecipe(List<Ingredient> ingredients)
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

        public List<Recipe> GetAllRecipes()
        {
            return _recipes.ToList();
        }
    }
}
