namespace CardManager.Domain.Repositories.User
{
    public interface IUserReadRepository
    {
        public Task<bool> ExistActiveUserWithEmail(string email);
        public Task<bool> ExistActiveUserWithId(long id);
        public Task<Entities.User?> GetEmail(string email);
        public Task<Entities.User?> GetUserWithId(long Id);
    }
}
