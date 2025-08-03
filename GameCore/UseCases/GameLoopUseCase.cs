using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.UseCases
{
    public class GameLoopUseCase
    {
        private readonly IClientService _clientService;
        private readonly EventBus _eventBus;
        private readonly GameState _gameState;

        public GameLoopUseCase(IClientService clientService, EventBus eventBus, GameState gameState)
        {
            _clientService = clientService;
            _eventBus = eventBus;
            _gameState = gameState;
        }

        public virtual Client StartNewRound()
        {
            var client = _clientService.GetNextClient();
            _gameState.StartNewRound(client);
            
            _eventBus.Publish(new ClientArrivedEvent(client));
            
            return client;
        }

        public virtual void SetPreparedDrink(Drink drink)
        {
            if (!_gameState.IsRoundActive)
                throw new InvalidOperationException("Nenhuma rodada ativa no momento.");

            _gameState.SetPreparedDrink(drink);
        }

        public virtual void CompleteRound(ClientReaction reaction)
        {
            if (!_gameState.IsRoundActive)
                throw new InvalidOperationException("Nenhuma rodada ativa no momento.");

            var scoreChange = CalculateScoreChange(reaction);
            _gameState.CompleteRound(scoreChange);
            
            _eventBus.Publish(new GameRoundCompletedEvent(_gameState.CurrentRound - 1));
        }

        public virtual GameState GetGameState()
        {
            return _gameState;
        }

        private int CalculateScoreChange(ClientReaction reaction)
        {
            return reaction switch
            {
                ClientReaction.VeryHappy => 100,
                ClientReaction.Happy => 75,
                ClientReaction.Neutral => 50,
                ClientReaction.Disappointed => 25,
                ClientReaction.Angry => 0,
                _ => 0
            };
        }
    }
}