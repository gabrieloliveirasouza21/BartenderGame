namespace Bartender.GameCore.Domain.Models
{
    public class GameState
    {
        public int CurrentRound { get; private set; }
        public int Money { get; private set; }
        public int TotalTipsEarned { get; private set; }
        public Client? CurrentClient { get; private set; }
        public Drink? PreparedDrink { get; private set; }
        public bool IsRoundActive { get; private set; }

        // Propriedade para compatibilidade com código existente
        public int Score => Money;

        public GameState()
        {
            CurrentRound = 1;
            Money = 0;
            TotalTipsEarned = 0;
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

        public void CompleteRound(int moneyEarned)
        {
            Money += moneyEarned;
            CurrentRound++;
            IsRoundActive = false;
            CurrentClient = null;
            PreparedDrink = null;
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public void AddTip(int tipAmount)
        {
            TotalTipsEarned += tipAmount;
        }

        public void ProcessPayment(PaymentResult paymentResult)
        {
            Money += paymentResult.TotalAmount;
            TotalTipsEarned += paymentResult.TipAmount;
        }

        public void SpendMoney(int amount)
        {
            if (amount > Money)
                throw new InvalidOperationException("Dinheiro insuficiente para realizar a compra.");
            
            Money -= amount;
        }

        public bool ShouldOpenShop()
        {
            return CurrentRound > 1 && (CurrentRound - 1) % 3 == 0;
        }

        // Método para compatibilidade com código existente
        public void AddScore(int points)
        {
            Money += points;
        }
    }
}