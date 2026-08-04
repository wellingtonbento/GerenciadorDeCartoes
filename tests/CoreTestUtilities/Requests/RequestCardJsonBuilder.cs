using Bogus;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public class RequestCardJsonBuilder
    {
        public static RequestCardJson Build()
        {
            return new Faker<RequestCardJson>()
                .RuleFor(card => card.Name, (f) => f.Person.FirstName)
                .RuleFor(card => card.CreditLimit, (f) => f.Finance.Amount(500, 10000))
                .RuleFor(card => card.CreditBalance, (f) => f.Finance.Amount(0, 5000))
                .RuleFor(card => card.Debit, (f) => f.Finance.Amount(0, 5000));
        }
    }
}
