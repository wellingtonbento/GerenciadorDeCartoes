namespace CardManager.Communication.Requests
{
    public class RequestCardJson
    {
        public string Name { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal Debit { get; set; }
    }
}
