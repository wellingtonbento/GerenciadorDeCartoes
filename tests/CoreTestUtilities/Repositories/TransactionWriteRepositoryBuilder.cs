using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.Transaction;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class TransactionWriteRepositoryBuilder
    {
        private readonly Mock<ITransactionWriteRepository> _repository;
        private Transaction? _capturedTransaction;

        public TransactionWriteRepositoryBuilder()
        {
            _repository = new Mock<ITransactionWriteRepository>();

            _repository.Setup(repository => repository.Add(It.IsAny<Transaction>()))
                .Callback<Transaction>(transaction => _capturedTransaction = transaction);
        }

        public Transaction? CapturedTransaction => _capturedTransaction;

        public void VerifyAddCalled() =>
            _repository.Verify(repository => repository.Add(It.IsAny<Transaction>()), Times.Once);

        public void VerifyAddNotCalled() =>
            _repository.Verify(repository => repository.Add(It.IsAny<Transaction>()), Times.Never);

        public ITransactionWriteRepository Build() => _repository.Object;
    }
}
