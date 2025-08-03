using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public interface IEvent { }

    public class DrinkPreparedEvent : IEvent
    {
        public Drink Drink { get; }
        public DrinkPreparedEvent(Drink drink) => Drink = drink;
    }
}
