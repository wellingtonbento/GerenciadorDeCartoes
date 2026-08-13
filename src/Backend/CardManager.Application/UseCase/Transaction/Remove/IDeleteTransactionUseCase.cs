namespace CardManager.Application.UseCase.Transaction.Remove
{
    public interface IDeleteTransactionUseCase
    {
        Task DeleteTransaction(long cardId, long transactionId);
    }
}
