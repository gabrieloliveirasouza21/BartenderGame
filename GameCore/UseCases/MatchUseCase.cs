using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.UseCases
{
    public class MatchUseCase
    {
        private readonly IGameModeService _gameModeService;
        private readonly IInventoryService _inventoryService;
        private readonly EventBus _eventBus;

        public MatchUseCase(IGameModeService gameModeService, IInventoryService inventoryService, EventBus eventBus)
        {
            _gameModeService = gameModeService;
            _inventoryService = inventoryService;
            _eventBus = eventBus;
        }

        public List<GameMode> GetAvailableGameModes()
        {
            return _gameModeService.GetAvailableGameModes();
        }

        public MatchState StartNewMatch(string gameModeId, GameState gameState)
        {
            var matchState = _gameModeService.CreateNewMatch(gameModeId);
            gameState.StartNewMatch(matchState);
            
            // Aplicar modificadores iniciais
            _gameModeService.ApplyStartingModifiers(matchState, _inventoryService);
            
            // Publicar evento de início de partida
            _eventBus.Publish(new MatchStartedEvent(matchState));
            
            return matchState;
        }

        public void EndMatch(GameState gameState)
        {
            if (gameState.CurrentMatch == null)
                throw new InvalidOperationException("Nenhuma partida ativa para finalizar.");

            var matchState = gameState.CurrentMatch;
            
            // Verificar se pode desbloquear próximo local
            if (matchState.CanUnlockNextLocation())
            {
                _gameModeService.UnlockNextLocation(matchState);
            }
            
            gameState.EndMatch();
            
            // Publicar evento de fim de partida
            _eventBus.Publish(new MatchCompletedEvent(matchState));
        }

        public PaymentResult ApplyMatchModifiers(MatchState matchState, PaymentResult basePayment, ClientReaction reaction)
        {
            return _gameModeService.ApplyPaymentModifiers(matchState, basePayment, reaction);
        }
    }
}