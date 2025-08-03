namespace Bartender.GameCore.Domain.Models
{
    public class Client
    {
        public string Name { get; }
        public string DesiredEffect { get; } // Ex: "Reconfortante"

        public Client(string name, string desiredEffect)
        {
            Name = name;
            DesiredEffect = desiredEffect;
        }
    }
}
