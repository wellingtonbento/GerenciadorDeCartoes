using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Transaction;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class TransactionRepository : ITransactionWriteRepository
    {
        private readonly CardManagerDbContext _context;

        public TransactionRepository(CardManagerDbContext context) => _context = context;

        public async Task Add(Transaction transaction) => await _context.Transactions.AddAsync(transaction);
    }
}
