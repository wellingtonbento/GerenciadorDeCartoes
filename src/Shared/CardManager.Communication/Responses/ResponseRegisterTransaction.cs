using CardManager.Communication.Enums;

namespace CardManager.Communication.Responses
{
    public class ResponseRegisterTransaction
    {
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}
