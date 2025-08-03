namespace Bartender.GameCore.Domain.Models
{
    public class Recipe
    {
        public string Name { get; }
        public List<string> RequiredIngredients { get; }
        public string Effect { get; }

        public Recipe(string name, List<string> requiredIngredients, string effect)
        {
            Name = name;
            RequiredIngredients = requiredIngredients;
            Effect = effect;
        }
    }
}
