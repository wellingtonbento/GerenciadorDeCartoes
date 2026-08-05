using CardManager.Application.UseCase.Card.Obtain;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Mapper;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Card.Obtain
{
    public class ObtainCardsUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var user = UserBuilder.Build().user;
            var cards = new List<CardManager.Domain.Entities.Card>
            {
                CardBuilder.Build(),
                CardBuilder.Build()
            };

            var repository = new CardReadRepositoryBuilder();
            repository.GetCards(cards);

            var useCase = CreateUseCase(user, repository);

            var result = await useCase.GetCards();

            result.Should().NotBeNull();
            result.Should().HaveCount(cards.Count);
            result[0].Name.Should().Be(cards[0].Name);
            result[0].CreditLimit.Should().Be(cards[0].CreditLimit);
            result[0].Debit.Should().Be(cards[0].Debit);
            result[0].AvailableCredit.Should().Be(cards[0].CreditLimit - cards[0].CreditBalance);
        }

        private static ObtainCardsUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardReadRepositoryBuilder repository)
        {
            var mapper = MapperBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);

            return new ObtainCardsUseCase(repository.Build(), loggedUser, mapper);
        }
    }
}
