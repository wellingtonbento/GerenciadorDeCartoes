using CardManager.Communication.Enums;

namespace CardManager.Communication.Responses
{
    public class ResponseObtainTransactionsJson
    {
        public DateTime CreatedOn { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
