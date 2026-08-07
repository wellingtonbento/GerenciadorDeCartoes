using Bogus;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public class RequestUpdateCardJsonBuilder
    {
        public static RequestUpdateCardJson Build()
        {
            return new Faker<RequestUpdateCardJson>()
                .RuleFor(card => card.Name, (f) => f.Person.FirstName)
                .RuleFor(card => card.CreditLimit, (f) => f.Finance.Amount(500, 10000))
                .RuleFor(card => card.Debit, (f) => f.Finance.Amount(0, 5000));
        }
    }
}
