using CardManager.Application.UseCase.User.Update;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.User.Update
{
    public class UpdateUserUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.UpdateUser(request);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.UpdateUser(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.NAME_REQUIRED));
        }

        [Fact]
        public async Task Error_Email_Empty()
        {
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.UpdateUser(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.EMAIL_REQUIRED));
        }

        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            (var user, _) = UserBuilder.Build();
            var request = RequestUpdateUserJsonBuilder.Build();

            var useCase = CreateUseCase(user, request.Email);

            Func<Task> act = async () => await useCase.UpdateUser(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.EMAIL_REQUIRED));
        }

        private static UpdateUserUseCase CreateUseCase(CardManager.Domain.Entities.User loggedUser, string? existEmail = null)
        {
            var loggedUserMock = LoggedUserBuilder.Build(loggedUser);
            var readRepositoryBuilder = new UserReadRepositoryBuilder();
            var updateRepository = UserUpdateRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            if (string.IsNullOrWhiteSpace(existEmail) == false)
                readRepositoryBuilder.ExistActiveUserWithEmail(existEmail);

            return new UpdateUserUseCase(loggedUserMock, readRepositoryBuilder.Build(), updateRepository, unitOfWork);
        }
    }
}
