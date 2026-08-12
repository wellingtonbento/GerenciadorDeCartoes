using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Transaction;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class TransactionReadRepositoryBuilder
    {
        private readonly Mock<ITransactionReadRepository> _repository;

        public TransactionReadRepositoryBuilder() => _repository = new Mock<ITransactionReadRepository>();

        public void ObtainTransactions(IList<Transaction> transactions)
        {
            _repository.Setup(repository => repository.ObtainTransactions(It.IsAny<long>())).ReturnsAsync(transactions);
        }

        public void ObtainTransaction(Transaction transaction)
        {
            _repository.Setup(repository => repository.ObtainTransaction(It.IsAny<long>())).ReturnsAsync(transaction);
        }

        public void VerifyObtainTransactionsCalled(long cardId) =>
            _repository.Verify(repository => repository.ObtainTransactions(cardId), Times.Once);

        public void VerifyObtainTransactionsNotCalled() =>
            _repository.Verify(repository => repository.ObtainTransactions(It.IsAny<long>()), Times.Never);

        public void VerifyObtainTransactionCalled(long id) =>
            _repository.Verify(repository => repository.ObtainTransaction(id), Times.Once);

        public void VerifyObtainTransactionNotCalled() =>
            _repository.Verify(repository => repository.ObtainTransaction(It.IsAny<long>()), Times.Never);

        public ITransactionReadRepository Build() => _repository.Object;
    }
}
