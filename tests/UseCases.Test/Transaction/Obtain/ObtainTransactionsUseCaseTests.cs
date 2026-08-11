using CardManager.Application.UseCase.Transaction.Obtain;
using CardManager.Domain.Entities;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Mapper;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Transaction.Obtain
{
    public class ObtainTransactionsUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var user = UserBuilder.Build().user;
            var card = CardBuilder.Build();
            card.UserId = user.Id;

            var transaction = TransactionBuilder.Build();
            transaction.CardId = card.Id;
            transaction.PaymentMethod = PaymentMethod.Debit;
            transaction.Amount = 100;
            transaction.Description = "Compra no mercado";

            var useCase = CreateUseCase(user, card, new List<CardManager.Domain.Entities.Transaction> { transaction }, out var readRepository);

            var result = await useCase.ObtainTransactions(card.Id);

            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result[0].Amount.Should().Be(100);
            result[0].Description.Should().Be("Compra no mercado");
            result[0].PaymentMethod.Should().Be(CardManager.Communication.Enums.PaymentMethod.Debit);

            readRepository.VerifyObtainTransactionsCalled(card.Id);
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var user = UserBuilder.Build().user;

            var useCase = CreateUseCase(user, null, new List<CardManager.Domain.Entities.Transaction>(), out var readRepository);

            Func<Task> act = async () => await useCase.ObtainTransactions(100);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));

            readRepository.VerifyObtainTransactionsNotCalled();
        }

        private static ObtainTransactionsUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardManager.Domain.Entities.Card? card, IList<CardManager.Domain.Entities.Transaction> transactions, out TransactionReadRepositoryBuilder readRepository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();

            var cardReadRepository = new CardReadRepositoryBuilder();
            if (card is not null)
                cardReadRepository.GetCard(card);

            readRepository = new TransactionReadRepositoryBuilder();
            readRepository.ObtainTransactions(transactions);

            return new ObtainTransactionsUseCase(readRepository.Build(), cardReadRepository.Build(), loggedUser, mapper);
        }
    }
}
