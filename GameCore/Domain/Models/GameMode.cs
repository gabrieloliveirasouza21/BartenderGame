using System.Collections.Generic;

namespace Bartender.GameCore.Domain.Models
{
    public class GameMode
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsUnlocked { get; private set; }
        public List<GameModeModifier> Modifiers { get; }
        public int DaysCount { get; } = 3; // Cada local tem 3 dias de trabalho
        public int RoundsPerDay { get; } = 10; // 10 rodadas por dia + 1 boss
        public string WorkingHours => "18:00 - 02:00"; // Horário de trabalho

        public GameMode(string id, string name, string description, bool isUnlocked, List<GameModeModifier> modifiers)
        {
            Id = id;
            Name = name;
            Description = description;
            IsUnlocked = isUnlocked;
            Modifiers = modifiers ?? new List<GameModeModifier>();
        }

        public void Unlock()
        {
            IsUnlocked = true;
        }
    }
}