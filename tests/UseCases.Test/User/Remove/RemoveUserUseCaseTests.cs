using CardManager.Application.UseCase.User.Remove;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.User.Remove
{
    public class RemoveUserUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.RemoveUser();

            await act.Should().NotThrowAsync();
        }

        private static RemoveUserUseCase CreateUseCase(CardManager.Domain.Entities.User loggedUser)
        {
            var loggedUserMock = LoggedUserBuilder.Build(loggedUser);
            var readRepositoryBuilder = new UserReadRepositoryBuilder();
            var deleteRepository = UserDeleteRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            readRepositoryBuilder.GetUserWithId(loggedUser);

            return new RemoveUserUseCase(loggedUserMock, deleteRepository, readRepositoryBuilder.Build(), unitOfWork);
        }
    }
}
