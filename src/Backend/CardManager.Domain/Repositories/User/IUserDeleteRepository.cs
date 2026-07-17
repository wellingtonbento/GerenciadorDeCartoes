namespace CardManager.Domain.Repositories.User
{
    public interface IUserDeleteRepository
    {
        public Task Delete(long Id);
    }
}
