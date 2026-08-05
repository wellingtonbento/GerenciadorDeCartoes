using Bogus;
using CardManager.Domain.Entities;

namespace CoreTestUtilities.Entities
{
    public class CardBuilder
    {
        public static Card Build()
        {
            return new Faker<Card>()
                .RuleFor(card => card.Id, (f) => f.Random.Long(1, 100))
                .RuleFor(card => card.UserId, (f) => f.Random.Long(1, 100))
                .RuleFor(card => card.Name, (f) => f.Person.FirstName)
                .RuleFor(card => card.CreditLimit, (f) => f.Finance.Amount(500, 10000))
                .RuleFor(card => card.CreditBalance, (f) => f.Finance.Amount(0, 5000))
                .RuleFor(card => card.Debit, (f) => f.Finance.Amount(0, 5000));
        }
    }
}
