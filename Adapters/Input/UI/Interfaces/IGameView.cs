using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Domain.Interfaces;

namespace Bartender.Adapters.Input.UI.Interfaces
{
    public interface IGameView
    {
        void DisplayWelcomeMessage();
        void DisplayClientArrival(Client client);
        void DisplayAvailableIngredients(List<Ingredient> ingredients);
        void DisplayAvailableIngredients(List<Ingredient> ingredients, IInventoryService inventoryService);
        List<Ingredient> GetSelectedIngredients(List<Ingredient> availableIngredients);
        bool ConfirmServeDrink(Drink drink);
        void DisplayClientReaction(string reactionMessage);
        void DisplayPaymentResult(PaymentResult paymentResult);
        void DisplayGameScore(int score, int round);
        bool AskToContinue();
        void DisplayGameOver();
    }
}