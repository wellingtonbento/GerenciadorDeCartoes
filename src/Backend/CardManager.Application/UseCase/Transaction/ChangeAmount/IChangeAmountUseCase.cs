using CardManager.Communication.Requests;

namespace CardManager.Application.UseCase.Transaction.ChangeAmount
{
    public interface IChangeAmountUseCase
    {
        Task ChangeAmount(long transactionId, long cardId, RequestChangeAmountJson request);
    }
}
