using CardManager.Communication.Enums;

namespace CardManager.Communication.Requests
{
    public class RequestCardJson
    {
        public string Name { get; set; } = string.Empty;
        public CardType Type { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal DebitBalance { get; set; }
    }
}
