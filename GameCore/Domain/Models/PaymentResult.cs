namespace Bartender.GameCore.Domain.Models
{
    public class PaymentResult
    {
        public int BaseAmount { get; }
        public int TipAmount { get; }
        public int TotalAmount { get; }
        public PaymentStatus Status { get; }
        public string PaymentMessage { get; }

        public PaymentResult(int baseAmount, int tipAmount, PaymentStatus status, string paymentMessage)
        {
            BaseAmount = baseAmount;
            TipAmount = tipAmount;
            TotalAmount = baseAmount + tipAmount;
            Status = status;
            PaymentMessage = paymentMessage;
        }
    }

    public enum PaymentStatus
    {
        Paid,
        PartiallyPaid,
        NotPaid
    }
}