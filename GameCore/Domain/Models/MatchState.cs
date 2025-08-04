namespace Bartender.GameCore.Domain.Models
{
    public class MatchState
    {
        public GameMode GameMode { get; }
        public int CurrentDay { get; private set; }
        public int CurrentClientInDay { get; private set; }
        public int TotalMoney { get; private set; }
        public int TotalTips { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsBossDay => CurrentDay == GameMode.DaysCount; // Último dia é o chefe

        public MatchState(GameMode gameMode)
        {
            GameMode = gameMode;
            CurrentDay = 1;
            CurrentClientInDay = 1;
            TotalMoney = 0;
            TotalTips = 0;
            IsCompleted = false;
        }

        public void NextClient()
        {
            CurrentClientInDay++;
            
            if (CurrentClientInDay > GameMode.ClientsPerDay)
            {
                NextDay();
            }
        }

        public void NextDay()
        {
            CurrentDay++;
            CurrentClientInDay = 1;
            
            if (CurrentDay > GameMode.DaysCount)
            {
                IsCompleted = true;
            }
        }

        public void AddMoney(int amount)
        {
            TotalMoney += amount;
        }

        public void AddTip(int amount)
        {
            TotalTips += amount;
        }

        public void SpendMoney(int amount)
        {
            if (amount > TotalMoney)
                throw new InvalidOperationException("Dinheiro insuficiente para realizar a compra.");
            
            TotalMoney -= amount;
        }

        public bool ShouldOpenShop()
        {
            // Loja abre a cada 3 clientes servidos
            return (CurrentDay - 1) * GameMode.ClientsPerDay + CurrentClientInDay - 1 > 0 && 
                   ((CurrentDay - 1) * GameMode.ClientsPerDay + CurrentClientInDay - 1) % 3 == 0;
        }

        public int GetTotalClientsServed()
        {
            return (CurrentDay - 1) * GameMode.ClientsPerDay + CurrentClientInDay - 1;
        }
    }
}