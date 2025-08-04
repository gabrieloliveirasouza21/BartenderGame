using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class ClientService : IClientService
    {
        private readonly List<Client> _clients;
        private readonly IBossClientService _bossClientService;
        private int _currentIndex = 0;

        public ClientService(IBossClientService? bossClientService = null)
        {
            _bossClientService = bossClientService ?? new BossClientService();
            _clients = new List<Client> {
                new Client("Alice", "Reconfortante"),
                new Client("Bob", "Energizante"),
                new Client("Clara", "Revigorante"),
                new Client("Diana", "Doce"),
                new Client("Eduardo", "Relaxante"),
                new Client("Fernanda", "Estimulante"),
                new Client("Gabriel", "Forte"),
                new Client("Helena", "Suave"),
                new Client("Igor", "Refrescante"),
                new Client("Julia", "Aromático"),
                new Client("Kevin", "Picante"),
                new Client("Lucia", "Neutro"),
                new Client("Marcos", "Natural"),
                new Client("Nina", "Ácido"),
                new Client("Otávio", "Mentolado")
            };
        }

        public Client GetNextClient()
        {
            var client = _clients[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _clients.Count;
            return client;
        }

        public Client GetBossClient()
        {
            return _bossClientService.GetMysteriousClient();
        }

        public Client GetClientForMatch(MatchState matchState, int roundNumber)
        {
            // Se é a rodada do boss (11ª rodada de cada dia), retorna o chefe
            if (matchState.IsBossRound)
            {
                return GetBossClient();
            }

            // Caso contrário, retorna um cliente normal
            return GetNextClient();
        }
    }
}
