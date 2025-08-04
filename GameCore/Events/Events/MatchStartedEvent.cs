using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class MatchStartedEvent : IEvent
    {
        public MatchState MatchState { get; }
        public DateTime Timestamp { get; }

        public MatchStartedEvent(MatchState matchState)
        {
            MatchState = matchState;
            Timestamp = DateTime.Now;
        }
    }
}