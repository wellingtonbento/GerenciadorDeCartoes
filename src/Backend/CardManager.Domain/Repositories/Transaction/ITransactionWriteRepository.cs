namespace CardManager.Domain.Repositories.Transaction
{
    public interface ITransactionWriteRepository
    {
        Task Add(Entities.Transaction transaction);
    }
}
