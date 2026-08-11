using CardManager.Communication.Enums;

namespace CardManager.Application.Services.Payment.Factory
{
    public interface IPaymentServiceFactory
    {
        IPaymentService GetService(PaymentMethod paymentMethod);
    }
}
