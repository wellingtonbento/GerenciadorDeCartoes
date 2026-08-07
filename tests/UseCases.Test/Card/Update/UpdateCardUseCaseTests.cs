using CardManager.Application.UseCase.Card.Update;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Card.Update
{
    public class UpdateCardUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditBalance = 0;
            var request = RequestUpdateCardJsonBuilder.Build();

            var useCase = CreateUseCase(user, card, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(card.Id, request);

            await act.Should().NotThrowAsync();

            updateRepository.VerifyUpdateCalled();
            updateRepository.CapturedCard.Should().NotBeNull();
            updateRepository.CapturedCard!.Name.Should().Be(request.Name);
            updateRepository.CapturedCard!.CreditLimit.Should().Be(request.CreditLimit);
            updateRepository.CapturedCard!.Debit.Should().Be(request.Debit);
            updateRepository.CapturedCard!.CreditBalance.Should().Be(card.CreditBalance);
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var user = UserBuilder.Build().user;
            var request = RequestUpdateCardJsonBuilder.Build();

            var useCase = CreateUseCase(user, null, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(1, request);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));

            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_CreditLimit_Lower_Than_Spent()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.CreditBalance = 1000;

            var request = RequestUpdateCardJsonBuilder.Build();
            request.CreditLimit = 999;

            var useCase = CreateUseCase(user, card, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(card.Id, request);

            (await act.Should().ThrowAsync<ErrorUpdateCardException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CREDIT_LIMIT_LOWER_THAN_THE_CREDIT_SPENT));

            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase(user, card, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.NAME_REQUIRED));

            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_CreditLimit_Negative()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.CreditLimit = -1;

            var useCase = CreateUseCase(user, card, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_CREDIT_LIMIT_INVALID));

            updateRepository.VerifyUpdateNotCalled();
        }

        [Fact]
        public async Task Error_Debit_Negative()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.Debit = -1;

            var useCase = CreateUseCase(user, card, out var updateRepository);

            Func<Task> act = async () => await useCase.UpdateCard(card.Id, request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_DEBIT_BALANCE_INVALID));

            updateRepository.VerifyUpdateNotCalled();
        }

        private static UpdateCardUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardManager.Domain.Entities.Card? card, out CardUpdateRepositoryBuilder updateRepository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);

            var readRepository = new CardReadRepositoryBuilder();
            if (card is not null)
                readRepository.GetCard(card);

            updateRepository = new CardUpdateRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new UpdateCardUseCase(loggedUser, readRepository.Build(), updateRepository.Build(), unitOfWork);
        }
    }
}
