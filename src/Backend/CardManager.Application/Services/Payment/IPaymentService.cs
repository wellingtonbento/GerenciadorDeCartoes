using CardManager.Domain.Entities;

namespace CardManager.Application.Services.Payment
{
    public interface IPaymentService
    {
        void Process(Card card, decimal amount);
    }
}
