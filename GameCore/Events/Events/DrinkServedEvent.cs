using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class DrinkServedEvent : IEvent
    {
        public Client Client { get; }
        public Drink Drink { get; }
        public DateTime ServedAt { get; }

        public DrinkServedEvent(Client client, Drink drink)
        {
            Client = client;
            Drink = drink;
            ServedAt = DateTime.Now;
        }
    }
}