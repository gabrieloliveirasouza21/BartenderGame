using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface IGameModeService
    {
        List<GameMode> GetAvailableGameModes();
        GameMode GetGameModeById(string id);
        MatchState CreateNewMatch(string gameModeId);
        void ApplyStartingModifiers(MatchState matchState, IInventoryService inventoryService);
        PaymentResult ApplyPaymentModifiers(MatchState matchState, PaymentResult basePayment, ClientReaction reaction);
    }
}