namespace Bartender.GameCore.Domain.Models
{
    public enum ModifierType
    {
        StartingIngredient,
        PaymentBonus,
        PaymentPenalty,
        SpecialEvent
    }

    public class GameModeModifier
    {
        public ModifierType Type { get; }
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, object> Parameters { get; }

        public GameModeModifier(ModifierType type, string name, string description, Dictionary<string, object>? parameters = null)
        {
            Type = type;
            Name = name;
            Description = description;
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        // Helper methods para acessar parâmetros tipados
        public T GetParameter<T>(string key, T defaultValue = default(T))
        {
            if (Parameters.ContainsKey(key) && Parameters[key] is T value)
                return value;
            return defaultValue;
        }
    }
}