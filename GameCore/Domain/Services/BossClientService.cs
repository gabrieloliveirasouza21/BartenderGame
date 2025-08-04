using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class BossClientService : IBossClientService
    {
        private readonly Random _random;

        public BossClientService()
        {
            _random = new Random();
        }

        public BossClient GetMysteriousClient()
        {
            var mysteriousClientData = GetMysteriousClientData();
            return new BossClient(
                mysteriousClientData.Name,
                mysteriousClientData.HiddenDesiredEffect,
                mysteriousClientData.Clues
            );
        }

        private (string Name, string HiddenDesiredEffect, List<string> Clues) GetMysteriousClientData()
        {
            var mysteriousClients = new List<(string Name, string HiddenDesiredEffect, List<string> Clues)>
            {
                (
                    "Figura Encapuzada",
                    "Reconfortante",
                    new List<string>
                    {
                        "Preciso de algo que aqueça minha alma...",
                        "Hoje foi um dia difícil, busco conforto.",
                        "Algo que me lembre de casa seria perfeito.",
                        "Meu coração pede calor e aconchego."
                    }
                ),
                (
                    "Homem de Terno",
                    "Energizante",
                    new List<string>
                    {
                        "Tenho uma longa noite pela frente...",
                        "Preciso de combustível para o que vem.",
                        "Algo que desperte meus sentidos.",
                        "A energia é o que busco neste momento."
                    }
                ),
                (
                    "Mulher Misteriosa",
                    "Doce",
                    new List<string>
                    {
                        "A vida tem sido amarga ultimamente...",
                        "Busco algo que traga doçura aos meus dias.",
                        "Preciso de um contraste para minha amargura.",
                        "Algo que me faça sorrir novamente."
                    }
                )
            };

            var selectedClient = mysteriousClients[_random.Next(mysteriousClients.Count)];
            return selectedClient;
        }
    }
}