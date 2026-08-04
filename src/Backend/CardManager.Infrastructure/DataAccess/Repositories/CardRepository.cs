using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Card;

namespace CardManager.Infrastructure.DataAccess.Repositories
{
    public class CardRepository : ICardWriteRepository
    {
        private readonly CardManagerDbContext _context;
        public CardRepository(CardManagerDbContext context) => _context = context;

        public async Task Add(Card card) => await _context.Cards.AddAsync(card);

    }
}
