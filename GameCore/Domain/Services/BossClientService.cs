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
                        "Preciso de algo que aque�a minha alma...",
                        "Hoje foi um dia dif�cil, busco conforto.",
                        "Algo que me lembre de casa seria perfeito.",
                        "Meu cora��o pede calor e aconchego."
                    }
                ),
                (
                    "Homem de Terno",
                    "Energizante",
                    new List<string>
                    {
                        "Tenho uma longa noite pela frente...",
                        "Preciso de combust�vel para o que vem.",
                        "Algo que desperte meus sentidos.",
                        "A energia � o que busco neste momento."
                    }
                ),
                (
                    "Mulher Misteriosa",
                    "Doce",
                    new List<string>
                    {
                        "A vida tem sido amarga ultimamente...",
                        "Busco algo que traga do�ura aos meus dias.",
                        "Preciso de um contraste para minha amargura.",
                        "Algo que me fa�a sorrir novamente."
                    }
                )
            };

            var selectedClient = mysteriousClients[_random.Next(mysteriousClients.Count)];
            return selectedClient;
        }
    }
}