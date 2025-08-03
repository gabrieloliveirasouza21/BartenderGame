using Bartender.Adapters.Input.UI.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.UseCases;

namespace Bartender.Adapters.Input.UI
{
    public class CraftPresenter
    {
        private readonly ICraftView _view;
        private readonly PrepareDrinkUseCase _useCase;

        public CraftPresenter(ICraftView view, PrepareDrinkUseCase useCase)
        {
            _view = view;
            _useCase = useCase;
        }

        public void OnPrepareDrink(List<Ingredient> ingredients)
        {
            _useCase.Execute(ingredients);
        }
    }
}
