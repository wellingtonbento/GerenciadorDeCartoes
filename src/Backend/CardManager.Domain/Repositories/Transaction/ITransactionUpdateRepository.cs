namespace CardManager.Domain.Repositories.Transaction
{
    public interface ITransactionUpdateRepository
    {
        Task UpdateAmount(long id, decimal amount); 
    }
}
