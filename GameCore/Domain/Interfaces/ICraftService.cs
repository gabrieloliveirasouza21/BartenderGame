using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface ICraftService
    {
        Drink Craft(List<Ingredient> ingredients);
    }

}
