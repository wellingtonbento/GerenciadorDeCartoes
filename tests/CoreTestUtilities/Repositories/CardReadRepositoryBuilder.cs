using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Card;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class CardReadRepositoryBuilder
    {
        private readonly Mock<ICardReadRepository> _repository;

        public CardReadRepositoryBuilder() => _repository = new Mock<ICardReadRepository>();

        public void GetCards(IList<Card> cards)
        {
            _repository.Setup(repository => repository.GetCards(It.IsAny<long>())).ReturnsAsync(cards);
        }

        public ICardReadRepository Build() => _repository.Object;
    }
}
