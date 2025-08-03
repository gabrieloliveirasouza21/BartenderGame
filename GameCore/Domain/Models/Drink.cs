namespace Bartender.GameCore.Domain.Models
{
    public class Drink
    {
        public string Name { get; }
        public List<Ingredient> Ingredients { get; }
        public string Effect { get; }

        public Drink(string name, List<Ingredient> ingredients, string effect)
        {
            Name = name;
            Ingredients = ingredients;
            Effect = effect;
        }
    }
}
