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
        
        // Novos campos para sistema de partidas
        public MatchState? CurrentMatch { get; private set; }
        public bool IsInMatch => CurrentMatch != null && !CurrentMatch.IsCompleted;
        public string? CurrentBossBonus { get; private set; }

        // Propriedade para compatibilidade com código existente
        public int Score => Money;

        public GameState()
        {
            CurrentRound = 1;
            Money = 0;
            TotalTipsEarned = 0;
            IsRoundActive = false;
            CurrentMatch = null;
            CurrentBossBonus = null;
        }

        public void StartNewMatch(MatchState matchState)
        {
            CurrentMatch = matchState;
            // Reset dos valores para a nova partida
            CurrentRound = 1;
            Money = 0;
            TotalTipsEarned = 0;
            CurrentBossBonus = null;
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
            
            // Atualizar o estado da partida
            CurrentMatch?.NextRound();
        }

        public void AddMoney(int amount)
        {
            Money += amount;
            CurrentMatch?.AddMoney(amount);
        }

        public void AddTip(int tipAmount)
        {
            TotalTipsEarned += tipAmount;
            CurrentMatch?.AddTip(tipAmount);
        }

        public void ProcessPayment(PaymentResult paymentResult)
        {
            Money += paymentResult.TotalAmount;
            TotalTipsEarned += paymentResult.TipAmount;
            CurrentMatch?.AddMoney(paymentResult.TotalAmount);
            CurrentMatch?.AddTip(paymentResult.TipAmount);
        }

        public void SpendMoney(int amount)
        {
            if (amount > Money)
                throw new InvalidOperationException("Dinheiro insuficiente para realizar a compra.");
            
            Money -= amount;
            CurrentMatch?.SpendMoney(amount);
        }

        public bool ShouldOpenShop()
        {
            if (CurrentMatch != null)
                return CurrentMatch.IsShopRound;
            
            // Fallback para código legado
            return CurrentRound > 1 && (CurrentRound - 1) % 3 == 0;
        }

        public void RecordBossSatisfaction(bool wasSatisfied)
        {
            CurrentMatch?.RecordBossSatisfaction(wasSatisfied);
        }

        public void SetBossBonus(string bonus)
        {
            CurrentBossBonus = bonus;
        }

        // Método para compatibilidade com código existente
        public void AddScore(int points)
        {
            Money += points;
        }

        public void EndMatch()
        {
            if (CurrentMatch != null)
            {
                // Salvar estatísticas da partida se necessário
                CurrentMatch = null;
                CurrentBossBonus = null;
            }
        }
    }
}