namespace Bartender.GameCore.Domain.Models
{
    public class BossClient : Client
    {
        public List<string> Clues { get; }
        public string HiddenDesiredEffect { get; }

        public BossClient(string name, string hiddenDesiredEffect, List<string> clues) 
            : base(name, "???") // Não revela o efeito desejado
        {
            HiddenDesiredEffect = hiddenDesiredEffect;
            Clues = clues ?? new List<string>();
        }

        public override string GetOrderMessage()
        {
            // Retorna uma das pistas aleatoriamente
            if (Clues.Count == 0)
                return "Hmm... surprenda-me.";
            
            var random = new Random();
            var clue = Clues[random.Next(Clues.Count)];
            return $"\"{clue}\"";
        }
    }
}