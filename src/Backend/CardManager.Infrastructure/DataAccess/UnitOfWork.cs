using CardManager.Domain.Repositories;

namespace CardManager.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CardManagerDbContext _dbContext;

        public UnitOfWork(CardManagerDbContext dbContext) => _dbContext = dbContext;

        public async Task SaveDb() => await _dbContext.SaveChangesAsync();
    }
}
