using CardManager.Application.UseCase.Card.Remove;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Card.Remove
{
    public class DeleteCardUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var useCase = CreateUseCase(deleteByIdReturn: true);

            Func<Task> act = async () => await useCase.DeleteCard(1);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            var useCase = CreateUseCase(deleteByIdReturn: false);

            Func<Task> act = async () => await useCase.DeleteCard(1);

            (await act.Should().ThrowAsync<NotFoundException>())
                .Where(error => error.GetErrorMessages().Contains(MessagesException.CARD_NOT_FOUND));
        }

        private static DeleteCardByIdUseCase CreateUseCase(bool deleteByIdReturn)
        {
            var user = UserBuilder.Build().user;
            var writeRepository = CardWriteRepositoryBuilder.Build(deleteByIdReturn);
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new DeleteCardByIdUseCase(writeRepository, loggedUser, unitOfWork);
        }
    }
}
