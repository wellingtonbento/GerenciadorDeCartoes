using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserReadRepository, IUserWriteRepository, IUserUpdateRepository, IUserDeleteRepository
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);

            user!.Password = passwordHash;
            await _dbContext.SaveChangesAsync();

        }

        public void UpdateProfile(User user)
        {
            _dbContext.Users.Attach(user);

            _dbContext.Entry(user).Property(user => user.Name).IsModified = true;
            _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
        }

        public async Task Delete(long Id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Active && user.Id == Id);

            _dbContext.Users.Remove(user!);
        }

        public async Task<User?> GetUserWithId(long Id) => await _dbContext.Users.FirstAsync(user => user.Active && user.Id == Id);
    }
}
