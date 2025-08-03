using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.UseCases
{
    public class PrepareDrinkUseCase
    {
        private readonly ICraftService _craftService;
        private readonly IInventoryService _inventoryService;
        private readonly EventBus _eventBus;

        public PrepareDrinkUseCase(ICraftService craftService, IInventoryService inventoryService, EventBus eventBus)
        {
            _craftService = craftService;
            _inventoryService = inventoryService;
            _eventBus = eventBus;
        }

        public void Execute(List<Ingredient> ingredients)
        {
            if (!_inventoryService.HasIngredients(ingredients))
            {
                Console.WriteLine("Ingredientes insuficientes!");
                return;
            }

            var drink = _craftService.Craft(ingredients);
            _inventoryService.ConsumeIngredients(ingredients);
            _eventBus.Publish(new DrinkPreparedEvent(drink));
        }
    }
}
