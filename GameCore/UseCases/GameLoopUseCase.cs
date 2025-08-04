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
                client = _clientService.GetClientForMatch(_gameState.CurrentMatch, _gameState.CurrentMatch.CurrentRoundInDay);
                
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

            // Se foi um boss client, registrar satisfação
            if (_gameState.CurrentClient is BossClient && _gameState.CurrentMatch != null)
            {
                bool wasSatisfied = reaction == ClientReaction.VeryHappy || reaction == ClientReaction.Happy;
                _gameState.RecordBossSatisfaction(wasSatisfied);
                
                // Se boss ficou satisfeito, aplicar bônus no próximo dia
                if (wasSatisfied && _gameState.CurrentMatch.CurrentDay < _gameState.CurrentMatch.GameMode.DaysCount)
                {
                    var bonus = _gameModeService.GetBossBonus(_gameState.CurrentMatch, _gameState.CurrentMatch.CurrentDay);
                    _gameState.SetBossBonus(bonus);
                }
            }

            // Publica evento de pagamento processado
            _eventBus.Publish(new PaymentProcessedEvent(_gameState.CurrentClient!, _gameState.PreparedDrink!, paymentResult));

            // Completa a rodada
            _gameState.CompleteRound(paymentResult.TotalAmount);

            // Publica evento de rodada completada
            _eventBus.Publish(new GameRoundCompletedEvent(_gameState.CurrentRound - 1));
        }

        public GameState GetGameState()
        {
            return _gameState;
        }
    }
}