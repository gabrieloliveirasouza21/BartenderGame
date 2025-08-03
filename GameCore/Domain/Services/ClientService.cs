using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class ClientService : IClientService
    {
        private readonly List<Client> _clients;
        private int _currentIndex = 0;

        public ClientService()
        {
            _clients = new List<Client> {
            new Client("Alice", "Reconfortante"),
            new Client("Bob", "Energizante"),
            new Client("Clara", "Revigorante")
        };
        }

        public Client GetNextClient()
        {
            var client = _clients[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _clients.Count;
            return client;
        }
    }
}
