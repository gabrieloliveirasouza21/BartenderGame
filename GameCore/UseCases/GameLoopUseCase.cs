using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;
using Bartender.GameCore.Domain.Services;

namespace Bartender.GameCore.UseCases
{
    public class GameLoopUseCase
    {
        private readonly IClientService _clientService;
        private readonly IPaymentService _paymentService;
        private readonly IGameModeService _gameModeService;
        private readonly EventBus _eventBus;
        private readonly GameState _gameState;
        private readonly int _baseDrinkPrice;

        public GameLoopUseCase(
            IClientService clientService, 
            IPaymentService paymentService, 
            EventBus eventBus, 
            GameState gameState, 
            IGameModeService? gameModeService = null,
            int baseDrinkPrice = 50)
        {
            _clientService = clientService;
            _paymentService = paymentService;
            _gameModeService = gameModeService ?? new GameModeService();
            _eventBus = eventBus;
            _gameState = gameState;
            _baseDrinkPrice = baseDrinkPrice;
        }

        public virtual Client StartNewRound()
        {
            Client client;
            
            if (_gameState.IsInMatch && _gameState.CurrentMatch != null)
            {
                // Usar o sistema de partidas para determinar o cliente
                client = _clientService.GetClientForMatch(_gameState.CurrentMatch, _gameState.CurrentMatch.CurrentClientInDay);
                
                // Se é um cliente chefe, publicar evento especial
                if (client is BossClient bossClient)
                {
                    _eventBus.Publish(new BossClientArrivedEvent(bossClient, _gameState.CurrentMatch.CurrentDay));
                }
            }
            else
            {
                // Fallback para o sistema antigo
                client = _clientService.GetNextClient();
            }
            
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

            var paymentResult = _paymentService.CalculatePayment(reaction, _baseDrinkPrice);
            
            // Aplicar modificadores da partida se estiver em uma partida
            if (_gameState.IsInMatch && _gameState.CurrentMatch != null)
            {
                paymentResult = _gameModeService.ApplyPaymentModifiers(_gameState.CurrentMatch, paymentResult, reaction);
            }
            
            _gameState.ProcessPayment(paymentResult);

            // Publica evento de pagamento processado
            if (_gameState.CurrentClient != null && _gameState.PreparedDrink != null)
            {
                _eventBus.Publish(new PaymentProcessedEvent(_gameState.CurrentClient, _gameState.PreparedDrink, paymentResult));
            }

            _gameState.CompleteRound(0); // Agora passamos 0 porque o pagamento já foi processado
            
            _eventBus.Publish(new GameRoundCompletedEvent(_gameState.CurrentRound - 1));
        }

        public virtual GameState GetGameState()
        {
            return _gameState;
        }

        public virtual bool IsMatchCompleted()
        {
            return _gameState.CurrentMatch?.IsCompleted ?? false;
        }

        // Método mantido para compatibilidade com testes existentes
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