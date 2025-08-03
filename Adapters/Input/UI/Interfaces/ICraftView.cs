using Bartender.GameCore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bartender.Adapters.Input.UI.Interfaces
{
    public interface ICraftView
    {
        void ShowDrinkResult(Drink drink);
    }
}
