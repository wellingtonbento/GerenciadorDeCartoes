namespace CardManager.Domain.Repositories.Transaction
{
    public interface ITransactionDeleteRepository
    {
        Task Delete(long id);
    }
}
