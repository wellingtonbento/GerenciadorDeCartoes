using CardManager.Domain.Repositories.Transaction;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class TransactionDeleteRepositoryBuilder
    {
        private readonly Mock<ITransactionDeleteRepository> _repository;

        public TransactionDeleteRepositoryBuilder() => _repository = new Mock<ITransactionDeleteRepository>();

        public void VerifyDeleteCalled(long id) =>
            _repository.Verify(repository => repository.Delete(id), Times.Once);

        public void VerifyDeleteNotCalled() =>
            _repository.Verify(repository => repository.Delete(It.IsAny<long>()), Times.Never);

        public ITransactionDeleteRepository Build() => _repository.Object;
    }
}
