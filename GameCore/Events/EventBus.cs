using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> _subscribers = new();

        public void Subscribe<T>(Action<IEvent> handler) where T : IEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Action<IEvent>>();
            _subscribers[type].Add(handler);
        }

        public void Publish(IEvent e)
        {
            var type = e.GetType();
            if (_subscribers.ContainsKey(type))
            {
                foreach (var handler in _subscribers[type])
                    handler(e);
            }
        }
    }
}
