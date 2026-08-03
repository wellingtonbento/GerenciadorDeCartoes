using CardManager.Domain.Enums;

namespace CardManager.Domain.Entities
{
    public class Card : EntityBase
    {
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CardType Type { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal DebitBalance { get; set; }

        public decimal AvailableCredit => CreditLimit - AmountSpent;
        public decimal TotalAvailable => AvailableCredit + DebitBalance;
    }
}
