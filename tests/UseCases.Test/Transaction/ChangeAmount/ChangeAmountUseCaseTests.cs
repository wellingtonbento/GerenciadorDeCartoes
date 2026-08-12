using CardManager.Application.Services.Payment.Factory;
using CardManager.Application.UseCase.Transaction.ChangeAmount;
using CardManager.Domain.Entities;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Transaction.ChangeAmount
{
    public class ChangeAmountUseCaseTests
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

            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = 150;

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            await useCase.ChangeAmount(transaction.Id, card.Id, request);

            cardUpdateRepository.VerifyUpdateCalled();
            cardUpdateRepository.CapturedCard!.Debit.Should().Be(950);

            updateRepository.VerifyUpdateAmountCalled(transaction.Id, 150);
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

            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = 150;

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            await useCase.ChangeAmount(transaction.Id, card.Id, request);

            cardUpdateRepository.VerifyUpdateCalled();
            cardUpdateRepository.CapturedCard!.CreditBalance.Should().Be(550);

            updateRepository.VerifyUpdateAmountCalled(transaction.Id, 150);
        }

        [Fact]
        public async Task Error_Transaction_Not_Found()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();

            var request = RequestChangeAmountJsonBuilder.Build();

            var useCase = CreateUseCase(user, card, null, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(99999, card.Id, request);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.TRANSACTION_NOT_FOUND));

            updateRepository.VerifyUpdateAmountNotCalled();
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

            var request = RequestChangeAmountJsonBuilder.Build();

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(transaction.Id, card.Id, request);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.TRANSACTION_NOT_FOUND));

            updateRepository.VerifyUpdateAmountNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var user = UserBuilder.Build().user;

            var transaction = TransactionBuilder.Build();
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var request = RequestChangeAmountJsonBuilder.Build();

            var useCase = CreateUseCase(user, null, transaction, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(transaction.Id, transaction.CardId, request);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));

            updateRepository.VerifyUpdateAmountNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Amount_Invalid()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = -1;

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(transaction.Id, card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CHANGE_AMOUNT));

            updateRepository.VerifyUpdateAmountNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Debit_Insufficient()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.Debit = 50;

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;

            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = 200;

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(transaction.Id, card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_DEBIT_INVALID));

            updateRepository.VerifyUpdateAmountNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Credit_Limit_Exceeded()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 950;

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Credit;
            transaction.Amount = 100;

            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = 200;

            var useCase = CreateUseCase(user, card, transaction, out var updateRepository, out var cardUpdateRepository);

            Func<Task> act = async () => await useCase.ChangeAmount(transaction.Id, card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_CREDIT_INVALID));

            updateRepository.VerifyUpdateAmountNotCalled();
            cardUpdateRepository.VerifyUpdateNotCalled();
        }

        private static ChangeAmountUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardManager.Domain.Entities.Card? card, CardManager.Domain.Entities.Transaction? transaction, out TransactionUpdateRepositoryBuilder updateRepository, out CardUpdateRepositoryBuilder cardUpdateRepository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);

            var readRepository = new TransactionReadRepositoryBuilder();
            if (transaction is not null)
                readRepository.ObtainTransaction(transaction);

            var cardReadRepository = new CardReadRepositoryBuilder();
            if (card is not null)
                cardReadRepository.GetCard(card);

            updateRepository = new TransactionUpdateRepositoryBuilder();
            cardUpdateRepository = new CardUpdateRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var paymentServiceFactory = new PaymentServiceFactory();

            return new ChangeAmountUseCase(readRepository.Build(), updateRepository.Build(), cardReadRepository.Build(), cardUpdateRepository.Build(), paymentServiceFactory, loggedUser, unitOfWork);
        }
    }
}
