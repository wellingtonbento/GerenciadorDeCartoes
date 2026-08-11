using CardManager.Communication.Enums;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.Services.Payment.Factory
{
    public class PaymentServiceFactory : IPaymentServiceFactory
    {
        public IPaymentService GetService(PaymentMethod paymentMethod)
        {
            return paymentMethod switch
            {
                PaymentMethod.Debit => new DebitPaymentService(),
                PaymentMethod.Credit => new CreditPaymentService(),
                _ => throw new ErrorOnValidationException(new List<string> { MessagesException.PAYMENT_METHOD_INVALID })
            };
        }
    }
}
