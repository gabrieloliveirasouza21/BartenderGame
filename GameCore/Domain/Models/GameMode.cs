using System.Collections.Generic;

namespace Bartender.GameCore.Domain.Models
{
    public class GameMode
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsUnlocked { get; }
        public List<GameModeModifier> Modifiers { get; }
        public int DaysCount { get; } = 7; // Cada partida tem 7 dias
        public int ClientsPerDay { get; } = 10; // 10 clientes por dia

        public GameMode(string id, string name, string description, bool isUnlocked, List<GameModeModifier> modifiers)
        {
            Id = id;
            Name = name;
            Description = description;
            IsUnlocked = isUnlocked;
            Modifiers = modifiers ?? new List<GameModeModifier>();
        }
    }
}