using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Card;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class CardRepository : ICardWriteRepository, ICardReadRepository
    {
        private readonly CardManagerDbContext _context;
        public CardRepository(CardManagerDbContext context) => _context = context;

        public async Task Add(Card card) => await _context.Cards.AddAsync(card);

        public async Task<IList<Card>> GetCards(long userId)
        {
            return await _context.Cards.AsNoTracking().Where(cards => cards.Active && cards.UserId == userId).ToListAsync();
        }
    }
}
