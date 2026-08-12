using Bogus;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public class RequestChangeAmountJsonBuilder
    {
        public static RequestChangeAmountJson Build()
        {
            return new Faker<RequestChangeAmountJson>()
                .RuleFor(request => request.Amount, (f) => f.Finance.Amount(1, 1000));
        }
    }
}
