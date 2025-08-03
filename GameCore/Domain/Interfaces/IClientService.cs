using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface IClientService
    {
        Client GetNextClient();
    }
}
