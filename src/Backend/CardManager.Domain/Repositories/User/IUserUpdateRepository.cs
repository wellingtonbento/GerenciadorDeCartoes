namespace CardManager.Domain.Repositories.User
{
    public interface IUserUpdateRepository
    {
        void UpdateProfile(Entities.User user);
        Task UpdatePassword(long userId, string passwordHash);

    }
}
