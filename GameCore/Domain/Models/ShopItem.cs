namespace Bartender.GameCore.Domain.Models
{
    public class ShopItem
    {
        public string Name { get; }
        public List<string> Tags { get; }
        public int Price { get; }
        public int Quantity { get; }
        public ShopItemType Type { get; }

        public ShopItem(string name, List<string> tags, int price, int quantity, ShopItemType type)
        {
            Name = name;
            Tags = tags;
            Price = price;
            Quantity = quantity;
            Type = type;
        }
    }

    public enum ShopItemType
    {
        NewIngredient,
        IngredientRestock
    }
}