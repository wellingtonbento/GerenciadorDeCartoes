using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Card;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class CardUpdateRepositoryBuilder
    {
        private readonly Mock<ICardUpdateRepository> _repository;
        private Card? _capturedCard;

        public CardUpdateRepositoryBuilder()
        {
            _repository = new Mock<ICardUpdateRepository>();

            _repository.Setup(repository => repository.Update(It.IsAny<Card>()))
                .Callback<Card>(card => _capturedCard = card);
        }

        public Card? CapturedCard => _capturedCard;

        public void VerifyUpdateCalled() =>
            _repository.Verify(repository => repository.Update(It.IsAny<Card>()), Times.Once);

        public void VerifyUpdateNotCalled() =>
            _repository.Verify(repository => repository.Update(It.IsAny<Card>()), Times.Never);

        public ICardUpdateRepository Build() => _repository.Object;
    }
}
