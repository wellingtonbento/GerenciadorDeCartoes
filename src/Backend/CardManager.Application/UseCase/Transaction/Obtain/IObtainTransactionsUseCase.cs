using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Transaction.Obtain
{
    public interface IObtainTransactionsUseCase
    {
        Task<IList<ResponseObtainTransactionsJson>> ObtainTransactions(long cardId);
    }
}
