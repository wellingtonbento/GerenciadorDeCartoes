namespace CardManager.Domain.Entities
{
    public class Transaction : EntityBase
    {
        public long CardId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}
