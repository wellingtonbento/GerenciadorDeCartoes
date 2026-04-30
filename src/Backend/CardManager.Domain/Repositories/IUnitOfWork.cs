namespace CardManager.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public Task SaveDb();
    }
}
