using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Transaction;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class TransactionRepository : ITransactionWriteRepository, ITransactionReadRepository
    {
        private readonly CardManagerDbContext _context;

        public TransactionRepository(CardManagerDbContext context) => _context = context;

        public async Task Add(Transaction transaction) => await _context.Transactions.AddAsync(transaction);

        public async Task<IList<Transaction>> ObtainTransactions(long cardId)
        {
            return await _context.Transactions.AsNoTracking()
                .Where(t => t.Active && t.CardId == cardId)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }
    }
}
