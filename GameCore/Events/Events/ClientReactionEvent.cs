using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class ClientReactionEvent : IEvent
    {
        public Client Client { get; }
        public Drink Drink { get; }
        public ClientReaction Reaction { get; }
        public string Message { get; }
        public DateTime ReactedAt { get; }

        public ClientReactionEvent(Client client, Drink drink, ClientReaction reaction, string message)
        {
            Client = client;
            Drink = drink;
            Reaction = reaction;
            Message = message;
            ReactedAt = DateTime.Now;
        }
    }
}