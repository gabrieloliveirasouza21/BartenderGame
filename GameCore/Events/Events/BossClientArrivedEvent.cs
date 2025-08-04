using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Events.Events
{
    public class BossClientArrivedEvent : IEvent
    {
        public BossClient BossClient { get; }
        public int Day { get; }
        public DateTime Timestamp { get; }

        public BossClientArrivedEvent(BossClient bossClient, int day)
        {
            BossClient = bossClient;
            Day = day;
            Timestamp = DateTime.Now;
        }
    }
}