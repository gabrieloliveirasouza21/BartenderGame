using Bartender.Adapters.Input.UI.Interfaces;
using Bartender.GameCore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bartender.Adapters.Input.UI
{
    public class ConsoleCraftView : ICraftView
    {
        public void ShowDrinkResult(Drink drink)
        {
            Console.WriteLine($"\nDrink preparado: {drink.Name} ({drink.Effect})\n");
        }
    }
}
