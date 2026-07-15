using CardManager.Domain.Entities;

namespace CardManager.Domain.Identity
{
    public interface ILoggedUser
    {
        Task<User> Get();
        long GetUserId();
    }
}
