namespace Bartender.GameCore.Events.Events
{
    public class ShopOpenedEvent : IEvent
    {
        public int Round { get; }
        
        public ShopOpenedEvent(int round)
        {
            Round = round;
        }
    }
}