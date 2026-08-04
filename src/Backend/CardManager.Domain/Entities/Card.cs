namespace CardManager.Domain.Entities
{
    public class Card : EntityBase
    {
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal Debit { get; set; }

        public decimal AvailableCredit => CreditLimit - CreditBalance;
        public decimal TotalAvailable => AvailableCredit + Debit;
    }
}
