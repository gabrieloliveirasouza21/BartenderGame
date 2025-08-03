using Bartender.Adapters.Input.UI;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Domain.Services;
using Bartender.GameCore.Events;
using Bartender.GameCore.UseCases;

namespace Bartender
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Configuração do inventário inicial
            var initialInventory = new Dictionary<string, int> {
                { "Café", 10 },
                { "Leite", 8 },
                { "Canela", 5 },
                { "Açúcar", 7 },
                { "Chocolate", 4 },
                { "Água", 15 }
            };

            // Inicialização dos serviços de domínio
            var recipeBook = new RecipeBook();
            var craftService = new CraftService(recipeBook);
            var inventoryService = new InventoryService(initialInventory);
            var clientService = new ClientService();
            var eventBus = new EventBus();
            var gameState = new GameState();

            // Inicialização dos casos de uso
            var prepareDrinkUseCase = new PrepareDrinkUseCase(craftService, inventoryService, eventBus);
            var serveClientUseCase = new ServeClientUseCase(eventBus);
            var gameLoopUseCase = new GameLoopUseCase(clientService, eventBus, gameState);

            // Inicialização da camada de apresentação
            var gameView = new ConsoleGameView();
            var gamePresenter = new GamePresenter(
                gameView,
                gameLoopUseCase,
                prepareDrinkUseCase,
                serveClientUseCase,
                inventoryService,
                eventBus);

            // Inicia o jogo
            gamePresenter.StartGame();
        }
    }
}
