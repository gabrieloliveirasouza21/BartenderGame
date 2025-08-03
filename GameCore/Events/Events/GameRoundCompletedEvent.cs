namespace Bartender.GameCore.Events.Events
{
    public class GameRoundCompletedEvent : IEvent
    {
        public int RoundNumber { get; }
        public DateTime CompletedAt { get; }

        public GameRoundCompletedEvent(int roundNumber)
        {
            RoundNumber = roundNumber;
            CompletedAt = DateTime.Now;
        }
    }
}