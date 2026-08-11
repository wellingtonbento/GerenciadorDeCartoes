using CardManager.Application.Services.Payment.Factory;
using CardManager.Application.UseCase.Transaction.Register;
using CardManager.Communication.Enums;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Mapper;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Transaction.Register
{
    public class RegisterTransactionUseCaseTests
    {
        [Fact]
        public async Task Success_Debit()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.Debit = 1000;

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Debit;
            request.Amount = 100;

            var useCase = CreateUseCase(user, card, out var writeRepository, out var updateRepository);

            var result = await useCase.RegisterTransaction(request);

            result.Should().NotBeNull();
            result.Amount.Should().Be(request.Amount);
            result.PaymentMethod.Should().Be(PaymentMethod.Debit);

            writeRepository.VerifyAddCalled();
            writeRepository.CapturedTransaction.Should().NotBeNull();
            writeRepository.CapturedTransaction!.CardId.Should().Be(card.Id);
            writeRepository.CapturedTransaction!.Amount.Should().Be(request.Amount);
            writeRepository.CapturedTransaction!.Description.Should().Be(request.Description);
            writeRepository.CapturedTransaction!.PaymentMethod.Should().Be(CardManager.Domain.Entities.PaymentMethod.Debit);

            updateRepository.VerifyUpdateCalled();
            updateRepository.CapturedCard.Should().NotBeNull();
            updateRepository.CapturedCard!.Debit.Should().Be(900);
        }

        [Fact]
        public async Task Success_Credit()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 500;

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Credit;
            request.Amount = 100;

            var useCase = CreateUseCase(user, card, out var writeRepository, out var updateRepository);

            var result = await useCase.RegisterTransaction(request);

            result.Should().NotBeNull();
            result.Amount.Should().Be(request.Amount);
            result.PaymentMethod.Should().Be(PaymentMethod.Credit);

            writeRepository.VerifyAddCalled();
            writeRepository.CapturedTransaction!.PaymentMethod.Should().Be(CardManager.Domain.Entities.PaymentMethod.Credit);

            updateRepository.VerifyUpdateCalled();
            updateRepository.CapturedCard.Should().NotBeNull();
            updateRepository.CapturedCard!.CreditBalance.Should().Be(600);
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var user = UserBuilder.Build().user;
            var request = RequestRegisterTransactionBuilder.Build();

            var useCase = CreateUseCase(user, null, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Debit_Insufficient()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.Debit = 50;

            var request = RequestRegisterTransactionBuilder.Build();
            request.PaymentMethod = PaymentMethod.Debit;
            request.Amount = 100;

            var useCase = CreateUseCase(user, card, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_DEBIT_INVALID));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Credit_Limit_Exceeded()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 950;

            var request = RequestRegisterTransactionBuilder.Build();
            request.PaymentMethod = PaymentMethod.Credit;
            request.Amount = 100;

            var useCase = CreateUseCase(user, card, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_CREDIT_INVALID));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_CardId_Invalid()
        {
            var user = UserBuilder.Build().user;
            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = 0;

            var useCase = CreateUseCase(user, null, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_ID_INVALID));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_PaymentMethod_Invalid()
        {
            var user = UserBuilder.Build().user;
            var request = RequestRegisterTransactionBuilder.Build();
            request.PaymentMethod = (PaymentMethod)99;

            var useCase = CreateUseCase(user, null, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.PAYMENT_METHOD_INVALID));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Amount_Invalid()
        {
            var user = UserBuilder.Build().user;
            var request = RequestRegisterTransactionBuilder.Build();
            request.Amount = 0;

            var useCase = CreateUseCase(user, null, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_VALUE_INVALID));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Description_Empty()
        {
            var user = UserBuilder.Build().user;
            var request = RequestRegisterTransactionBuilder.Build();
            request.Description = string.Empty;

            var useCase = CreateUseCase(user, null, out var writeRepository, out var updateRepository);

            Func<Task> act = async () => await useCase.RegisterTransaction(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.DESCRIPTION_REQUIRED));

            writeRepository.VerifyAddNotCalled();
            updateRepository.VerifyUpdateNotCalled();
        }

        private static RegisterTransactionUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardManager.Domain.Entities.Card? card, out TransactionWriteRepositoryBuilder writeRepository, out CardUpdateRepositoryBuilder updateRepository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();

            var readRepository = new CardReadRepositoryBuilder();
            if (card is not null)
                readRepository.GetCard(card);

            writeRepository = new TransactionWriteRepositoryBuilder();
            updateRepository = new CardUpdateRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var paymentServiceFactory = new PaymentServiceFactory();

            return new RegisterTransactionUseCase(writeRepository.Build(), readRepository.Build(), paymentServiceFactory, updateRepository.Build(), loggedUser, mapper, unitOfWork);
        }
    }
}
