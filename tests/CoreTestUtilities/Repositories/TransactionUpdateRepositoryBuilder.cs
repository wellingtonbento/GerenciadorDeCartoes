using CardManager.Domain.Repositories.Transaction;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class TransactionUpdateRepositoryBuilder
    {
        private readonly Mock<ITransactionUpdateRepository> _repository;

        public TransactionUpdateRepositoryBuilder() => _repository = new Mock<ITransactionUpdateRepository>();

        public void VerifyUpdateAmountCalled(long id, decimal amount) =>
            _repository.Verify(repository => repository.UpdateAmount(id, amount), Times.Once);

        public void VerifyUpdateAmountNotCalled() =>
            _repository.Verify(repository => repository.UpdateAmount(It.IsAny<long>(), It.IsAny<decimal>()), Times.Never);

        public ITransactionUpdateRepository Build() => _repository.Object;
    }
}
