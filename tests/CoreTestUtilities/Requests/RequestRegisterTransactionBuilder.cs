using Bogus;
using CardManager.Communication.Enums;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public class RequestRegisterTransactionBuilder
    {
        public static RequestRegisterTransaction Build()
        {
            return new Faker<RequestRegisterTransaction>()
                .RuleFor(transaction => transaction.CardId, (f) => f.Random.Long(1, 1000))
                .RuleFor(transaction => transaction.PaymentMethod, (f) => f.PickRandom<PaymentMethod>())
                .RuleFor(transaction => transaction.Amount, (f) => f.Finance.Amount(10, 1000))
                .RuleFor(transaction => transaction.Description, (f) => f.Lorem.Sentence());
        }
    }
}
