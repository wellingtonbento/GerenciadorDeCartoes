using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserReadRepository, IUserWriteRepository, IUserUpdateRepository
    {
        private readonly CardManagerDbContext _dbContext;

        public UserRepository(CardManagerDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(User user) => await _dbContext.Users.AddAsync(user);
        public async Task<bool> ExistActiveUserWithEmail(string email) => await _dbContext.Users.AnyAsync(user => user.Email!.Equals(email) && user.Active);

        public async Task<bool> ExistActiveUserWithId(long id) => await _dbContext.Users.AnyAsync(user => user.Active && user.Id == id);

        public async Task<User?> GetEmail(string email)
        {
            return await _dbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Active && user.Email.Equals(email));
        }

        public async Task UpdatePassword(long userId, string passwordHash)
        {
            await _dbContext
                .Users
                .Where(user => user.Id == userId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.Password, passwordHash));
        }

        public void UpdateProfile(User user)
        {
            _dbContext.Users.Attach(user);

            _dbContext.Entry(user).Property(user => user.Name).IsModified = true;
            _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
        }
    }
}
