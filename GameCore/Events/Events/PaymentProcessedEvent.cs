using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;

namespace Bartender.GameCore.Events.Events
{
    public class PaymentProcessedEvent : IEvent
    {
        public Client Client { get; }
        public Drink Drink { get; }
        public PaymentResult PaymentResult { get; }
        public DateTime Timestamp { get; }

        public PaymentProcessedEvent(Client client, Drink drink, PaymentResult paymentResult)
        {
            Client = client;
            Drink = drink;
            PaymentResult = paymentResult;
            Timestamp = DateTime.UtcNow;
        }
    }
}