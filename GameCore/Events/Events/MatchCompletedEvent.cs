using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class MatchCompletedEvent : IEvent
    {
        public MatchState MatchState { get; }
        public DateTime Timestamp { get; }

        public MatchCompletedEvent(MatchState matchState)
        {
            MatchState = matchState;
            Timestamp = DateTime.Now;
        }
    }
}