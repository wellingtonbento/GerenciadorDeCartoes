using CardManager.Communication.Enums;

namespace CardManager.Communication.Requests
{
    public class RequestRegisterTransaction
    {
        public long CardId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
