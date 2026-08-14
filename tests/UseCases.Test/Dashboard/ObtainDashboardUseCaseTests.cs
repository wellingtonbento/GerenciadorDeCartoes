using CardManager.Application.UseCase.Dashboard;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Dashboard
{
    public class ObtainDashboardUseCaseTests
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

            var result = await useCase.DashBoard();

            result.Should().NotBeNull();
            result.CreditLimit.Should().Be(cards.Sum(card => card.CreditLimit));
            result.AvailableCredit.Should().Be(cards.Sum(card => card.AvailableCredit));
            result.Debit.Should().Be(cards.Sum(card => card.Debit));
        }

        [Fact]
        public async Task Success_Without_Cards()
        {
            var user = UserBuilder.Build().user;

            var repository = new CardReadRepositoryBuilder();
            repository.GetCards(new List<CardManager.Domain.Entities.Card>());

            var useCase = CreateUseCase(user, repository);

            var result = await useCase.DashBoard();

            result.Should().NotBeNull();
            result.CreditLimit.Should().Be(0);
            result.AvailableCredit.Should().Be(0);
            result.Debit.Should().Be(0);
        }

        private static ObtainDashboardUseCase CreateUseCase(CardManager.Domain.Entities.User user, CardReadRepositoryBuilder repository)
        {
            var loggedUser = LoggedUserBuilder.Build(user);

            return new ObtainDashboardUseCase(repository.Build(), loggedUser);
        }
    }
}
