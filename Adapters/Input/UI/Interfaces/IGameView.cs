using Bartender.GameCore.Domain.Models;

namespace Bartender.Adapters.Input.UI.Interfaces
{
    public interface IGameView
    {
        void DisplayWelcomeMessage();
        void DisplayClientArrival(Client client);
        void DisplayAvailableIngredients(List<Ingredient> ingredients);
        List<Ingredient> GetSelectedIngredients(List<Ingredient> availableIngredients);
        bool ConfirmServeDrink(Drink drink);
        void DisplayClientReaction(string reactionMessage);
        void DisplayGameScore(int score, int round);
        bool AskToContinue();
        void DisplayGameOver();
    }
}