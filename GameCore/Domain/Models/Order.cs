namespace Bartender.GameCore.Domain.Models
{
    public class Order
    {
        public Client Client { get; }

        public Order(Client client)
        {
            Client = client;
        }
    }
}
