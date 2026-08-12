namespace CardManager.Domain.Repositories.Transaction
{
    public interface ITransactionReadRepository
    {
        Task<IList<Entities.Transaction>> ObtainTransactions(long cardId);
        Task<Entities.Transaction> ObtainTransaction(long id);
    }
}
