using Bartender.Adapters.Input.UI;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Domain.Services;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;
using Bartender.GameCore.UseCases;

namespace Bartender
{
    public class Program
    {
        static void Main(string[] args)
        {
            var recipeBook = new RecipeBook();
            var craftService = new CraftService(recipeBook);
            var inventory = new InventoryService(new Dictionary<string, int> {
            { "Café", 5 }, { "Leite", 3 }, { "Canela", 2 }
        });
            var eventBus = new EventBus();
            var useCase = new PrepareDrinkUseCase(craftService, inventory, eventBus);
            var view = new ConsoleCraftView();
            var presenter = new CraftPresenter(view, useCase);

            eventBus.Subscribe<DrinkPreparedEvent>(e => {
                var evt = (DrinkPreparedEvent)e;
                Console.WriteLine($"Evento: Drink '{evt.Drink.Name}' preparado com sucesso!");
            });

            var clientService = new ClientService();
            var client = clientService.GetNextClient();
            Console.WriteLine($"Cliente: {client.Name}, deseja um drink com efeito: {client.DesiredEffect}\n");

            var ingredients = new List<Ingredient> {
            new Ingredient("Café", new List<string> { "Amargo" }),
            new Ingredient("Leite", new List<string> { "Doce" }),
            new Ingredient("Canela", new List<string> { "Doce" })
        };

            presenter.OnPrepareDrink(ingredients);
        }
    }
}
