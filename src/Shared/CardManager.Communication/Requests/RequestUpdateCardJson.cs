namespace CardManager.Communication.Requests
{
    public class RequestUpdateCardJson
    {
        public string Name { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal Debit { get; set; }
    }
}
