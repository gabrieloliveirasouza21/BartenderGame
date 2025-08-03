using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Random _random;

        public PaymentService()
        {
            _random = new Random();
        }

        public virtual PaymentResult CalculatePayment(ClientReaction reaction, int baseDrinkPrice)
        {
            return reaction switch
            {
                ClientReaction.VeryHappy => CalculateVeryHappyPayment(baseDrinkPrice),
                ClientReaction.Happy => CalculateHappyPayment(baseDrinkPrice),
                ClientReaction.Neutral => CalculateNeutralPayment(baseDrinkPrice),
                ClientReaction.Disappointed => CalculateDisappointedPayment(baseDrinkPrice),
                ClientReaction.Angry => CalculateAngryPayment(baseDrinkPrice),
                _ => new PaymentResult(0, 0, PaymentStatus.NotPaid, "Reação desconhecida.")
            };
        }

        private PaymentResult CalculateVeryHappyPayment(int baseDrinkPrice)
        {
            var tipPercentage = 20; // 20% de gorjeta
            var tipAmount = (int)(baseDrinkPrice * tipPercentage / 100.0);
            
            return new PaymentResult(
                baseDrinkPrice, 
                tipAmount, 
                PaymentStatus.Paid,
                $"Cliente muito satisfeito! Pagou ${baseDrinkPrice} + ${tipAmount} de gorjeta (20%)");
        }

        private PaymentResult CalculateHappyPayment(int baseDrinkPrice)
        {
            var tipPercentage = 20; // 20% de gorjeta
            var tipAmount = (int)(baseDrinkPrice * tipPercentage / 100.0);
            
            return new PaymentResult(
                baseDrinkPrice, 
                tipAmount, 
                PaymentStatus.Paid,
                $"Cliente satisfeito! Pagou ${baseDrinkPrice} + ${tipAmount} de gorjeta (20%)");
        }

        private PaymentResult CalculateNeutralPayment(int baseDrinkPrice)
        {
            var halfPrice = baseDrinkPrice / 2;
            
            return new PaymentResult(
                halfPrice, 
                0, 
                PaymentStatus.PartiallyPaid,
                $"Cliente neutro. Pagou apenas ${halfPrice} (50% do valor)");
        }

        private PaymentResult CalculateDisappointedPayment(int baseDrinkPrice)
        {
            var tenPercentChance = _random.NextDouble() < 0.1; // 10% de chance de não pagar nada
            
            if (tenPercentChance)
            {
                return new PaymentResult(
                    0, 
                    0, 
                    PaymentStatus.NotPaid,
                    "Cliente insatisfeito se recusou a pagar!");
            }
            
            var tenPercent = (int)(baseDrinkPrice * 0.1);
            
            return new PaymentResult(
                tenPercent, 
                0, 
                PaymentStatus.PartiallyPaid,
                $"Cliente insatisfeito. Pagou apenas ${tenPercent} (10% do valor)");
        }

        private PaymentResult CalculateAngryPayment(int baseDrinkPrice)
        {
            return new PaymentResult(
                0, 
                0, 
                PaymentStatus.NotPaid,
                "Cliente furioso se recusou a pagar!");
        }
    }
}