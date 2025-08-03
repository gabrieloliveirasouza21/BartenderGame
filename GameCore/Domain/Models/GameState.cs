namespace Bartender.GameCore.Domain.Models
{
    public class GameState
    {
        public int CurrentRound { get; private set; }
        public int Score { get; private set; }
        public Client? CurrentClient { get; private set; }
        public Drink? PreparedDrink { get; private set; }
        public bool IsRoundActive { get; private set; }

        public GameState()
        {
            CurrentRound = 1;
            Score = 0;
            IsRoundActive = false;
        }

        public void StartNewRound(Client client)
        {
            CurrentClient = client;
            PreparedDrink = null;
            IsRoundActive = true;
        }

        public void SetPreparedDrink(Drink drink)
        {
            PreparedDrink = drink;
        }

        public void CompleteRound(int scoreChange)
        {
            Score += scoreChange;
            CurrentRound++;
            IsRoundActive = false;
            CurrentClient = null;
            PreparedDrink = null;
        }

        public void AddScore(int points)
        {
            Score += points;
        }
    }
}