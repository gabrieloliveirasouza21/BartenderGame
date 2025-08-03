using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class ClientArrivedEvent : IEvent
    {
        public Client Client { get; }
        public ClientArrivedEvent(Client client) => Client = client;
    }
}
