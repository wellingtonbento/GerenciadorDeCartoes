using CardManager.Domain.Entities;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.Services.Payment
{
    public class CreditPaymentService : IPaymentService
    {
        public void Process(Card card, decimal amount)
        {
            if (card.CreditBalance + amount > card.CreditLimit)
                throw new ErrorOnValidationException(new List<string> { MessagesException.TRANSACTION_CREDIT_INVALID });

            card.CreditBalance += amount;
        }
    }
}
