using CardManager.Application.UseCase.User.ChangePassword;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.User.ChangePassword
{
    public class ChangePasswordUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = password;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.ChangePassword(request);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Error_Current_Password_Incorrect()
        {
            (var user, _) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = "wrong_password";

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.ChangePassword(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.VALIDATION_CURRENT_PASSWORD));
        }

        [Fact]
        public async Task Error_New_Password_Empty()
        {
            (var user, var password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = password;
            request.NewPassword = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.ChangePassword(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.PASSWORD_REQUIRED));
        }

        [Fact]
        public async Task Error_New_Password_Too_Short()
        {
            (var user, var password) = UserBuilder.Build();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = password;
            request.NewPassword = "abc";

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.ChangePassword(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1
                    && error.GetErrorMessages().Contains(MessagesException.PASSWORD_EMPTY));
        }

        private static ChangePasswordUseCase CreateUseCase(CardManager.Domain.Entities.User loggedUser)
        {
            var loggedUserMock = LoggedUserBuilder.Build(loggedUser);
            var updateRepository = UserUpdateRepositoryBuilder.Build();

            return new ChangePasswordUseCase(loggedUserMock, updateRepository);
        }
    }
}
