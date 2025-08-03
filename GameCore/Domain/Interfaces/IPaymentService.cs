using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Interfaces
{
    public interface IPaymentService
    {
        PaymentResult CalculatePayment(ClientReaction reaction, int baseDrinkPrice);
    }
}