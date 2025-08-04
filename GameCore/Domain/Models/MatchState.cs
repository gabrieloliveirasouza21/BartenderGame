namespace Bartender.GameCore.Domain.Models
{
    public class MatchState
    {
        public GameMode GameMode { get; }
        public int CurrentDay { get; private set; }
        public int CurrentRoundInDay { get; private set; }
        public int TotalMoney { get; private set; }
        public int TotalTips { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsBossRound => CurrentRoundInDay == 11; // Boss é a 11ª "rodada" do dia
        public bool IsShopRound => CurrentRoundInDay > 0 && CurrentRoundInDay % 3 == 0 && CurrentRoundInDay < 10; // A cada 3 rodadas, mas não na 10ª
        public List<bool> BossSatisfactionByDay { get; private set; }
        public string CurrentTime { get; private set; }

        public MatchState(GameMode gameMode)
        {
            GameMode = gameMode;
            CurrentDay = 1;
            CurrentRoundInDay = 1;
            TotalMoney = 0;
            TotalTips = 0;
            IsCompleted = false;
            BossSatisfactionByDay = new List<bool>();
            CurrentTime = CalculateCurrentTime();
        }

        public void NextRound()
        {
            if (IsBossRound)
            {
                // Após o boss, avança para o próximo dia
                NextDay();
            }
            else if (CurrentRoundInDay >= GameMode.RoundsPerDay)
            {
                // Após a 10ª rodada, vem o boss
                CurrentRoundInDay = 11; // Boss round
            }
            else
            {
                CurrentRoundInDay++;
            }
            
            CurrentTime = CalculateCurrentTime();
        }

        public void NextDay()
        {
            CurrentDay++;
            CurrentRoundInDay = 1;
            
            if (CurrentDay > GameMode.DaysCount)
            {
                IsCompleted = true;
            }
            
            CurrentTime = CalculateCurrentTime();
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

        public void RecordBossSatisfaction(bool wasSatisfied)
        {
            if (BossSatisfactionByDay.Count < CurrentDay)
            {
                BossSatisfactionByDay.Add(wasSatisfied);
            }
        }

        public bool CanUnlockNextLocation()
        {
            // Precisa satisfazer pelo menos 2 dos 3 bosses
            int satisfiedBosses = BossSatisfactionByDay.Count(satisfied => satisfied);
            return satisfiedBosses >= 2;
        }

        public int GetTotalRoundsCompleted()
        {
            return (CurrentDay - 1) * GameMode.RoundsPerDay + CurrentRoundInDay - 1;
        }

        private string CalculateCurrentTime()
        {
            // Início às 18:00, cada rodada = 48 minutos (8 horas = 480 min / 10 rodadas)
            // Boss round seria próximo das 02:00
            int startHour = 18;
            int minutesPerRound = 48;
            
            if (IsBossRound)
            {
                return "02:00";
            }
            
            int totalMinutes = (CurrentRoundInDay - 1) * minutesPerRound;
            int currentHour = (startHour + totalMinutes / 60) % 24;
            int currentMinute = totalMinutes % 60;
            
            return $"{currentHour:D2}:{currentMinute:D2}";
        }
    }
}