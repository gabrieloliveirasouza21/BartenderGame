namespace Bartender.GameCore.Domain.Models
{
    public class Ingredient
    {
        public string Name { get; }
        public List<string> Tags { get; }

        public Ingredient(string name, List<string> tags)
        {
            Name = name;
            Tags = tags;
        }
    }
}
