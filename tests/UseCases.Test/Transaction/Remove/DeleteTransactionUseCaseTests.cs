using CardManager.Application.Services.Payment.Factory;
using CardManager.Application.UseCase.Transaction.Remove;
using CardManager.Domain.Entities;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Transaction.Remove
{
    public class DeleteTransactionUseCaseTests
    {
        [Fact]
        public async Task Success_Debit()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.Debit = 1000;

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var useCase = CreateUseCase(user, card, transaction, out var deleteRepository, out var cardUpdateRepository);

            await useCase.DeleteTransaction(card.Id, transaction.Id);

            cardUpdateRepository.VerifyUpdateCalled();
            cardUpdateRepository.CapturedCard!.Debit.Should().Be(1100);

            deleteRepository.VerifyDeleteCalled(transaction.Id);
        }

        [Fact]
        public async Task Success_Credit()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 500;

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Credit;
            transaction.Amount = 100;

            var useCase = CreateUseCase(user, card, transaction, out var deleteRepository, out var cardUpdateRepository);

            await useCase.DeleteTransaction(card.Id, transaction.Id);

            cardUpdateRepository.VerifyUpdateCalled();
            cardUpdateRepository.CapturedCard!.CreditBalance.Should().Be(400);

            deleteRepository.VerifyDeleteCalled(transaction.Id);
        }

        [Fact]
        public async Task Error_Transaction_Not_Found()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();

            var useCase = CreateUseCase(user, card, null, out var deleteRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.DeleteTransaction(card.Id, 99999);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.TRANSACTION_NOT_FOUND));

            deleteRepository.VerifyDeleteNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Transaction_Does_Not_Belong_To_Card()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();

            var transaction = TransactionBuilder.Build();
            transaction.CardId = 99999;
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var useCase = CreateUseCase(user, card, transaction, out var deleteRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.DeleteTransaction(card.Id, transaction.Id);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.TRANSACTION_NOT_FOUND));

            deleteRepository.VerifyDeleteNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var user = UserBuilder.Build().user;

            var transaction = TransactionBuilder.Build();
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var useCase = CreateUseCase(user, null, transaction, out var deleteRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.DeleteTransaction(transaction.CardId, transaction.Id);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));

            deleteRepository.VerifyDeleteNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        private static DeleteTransactionUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardManager.Domain.Entities.Card? card, CardManager.Domain.Entities.Transaction? transaction, out TransactionDeleteRepositoryBuilder deleteRepository, out CardUpdateRepositoryBuilder cardUpdateRepository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);

            var readRepository = new TransactionReadRepositoryBuilder();
            if (transaction is not null)
                readRepository.ObtainTransaction(transaction);

            var cardReadRepository = new CardReadRepositoryBuilder();
            if (card is not null)
                cardReadRepository.GetCard(card);

            deleteRepository = new TransactionDeleteRepositoryBuilder();
            cardUpdateRepository = new CardUpdateRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var paymentServiceFactory = new PaymentServiceFactory();

            return new DeleteTransactionUseCase(readRepository.Build(), deleteRepository.Build(), cardReadRepository.Build(), cardUpdateRepository.Build(), paymentServiceFactory, loggedUser, unitOfWork);
        }
    }
}
