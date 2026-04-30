namespace CardManager.Domain.Repositories.User
{
    public interface IUserWriteRepository
    {
        public Task Add(Entities.User user);
    }
}
