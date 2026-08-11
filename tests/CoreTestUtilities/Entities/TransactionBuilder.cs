using Bogus;
using CardManager.Domain.Entities;

namespace CoreTestUtilities.Entities
{
    public class TransactionBuilder
    {
        private static long _lastId;
        public static Transaction Build()
        {
            var id = Interlocked.Increment(ref _lastId);

            return new Faker<Transaction>()
                .RuleFor(transaction => transaction.Id, (f) => id)
                .RuleFor(transaction => transaction.CardId, (f) => f.Random.Long(1, 1000))
                .RuleFor(transaction => transaction.PaymentMethod, (f) => f.PickRandom<PaymentMethod>())
                .RuleFor(transaction => transaction.Amount, (f) => f.Finance.Amount(10, 1000))
                .RuleFor(transaction => transaction.Description, (f) => f.Lorem.Sentence());
        }
    }
}
