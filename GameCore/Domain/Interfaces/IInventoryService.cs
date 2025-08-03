using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface IInventoryService
    {
        List<Ingredient> GetAvailableIngredients();
        void ConsumeIngredients(List<Ingredient> usedIngredients);
        bool HasIngredients(List<Ingredient> needed);
    }
}
