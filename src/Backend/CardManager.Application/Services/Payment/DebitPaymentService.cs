using CardManager.Domain.Entities;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.Services.Payment
{
    public class DebitPaymentService : IPaymentService
    {
        public void Process(Card card, decimal amount)
        {
            if (card.Debit - amount < 0)
                throw new ErrorOnValidationException(new List<string> { MessagesException.TRANSACTION_DEBIT_INVALID });

            card.Debit -= amount;
        }
    }
}
