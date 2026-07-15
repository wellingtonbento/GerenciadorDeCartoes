using CardManager.Application.UseCase.Login;
using CardManager.Communication.Requests;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Test.Login
{
    public class LoginUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateLoginUseCase(user);

            var result = await useCase.Login(new RequestLoginJson
            {
                Email = user.Email,
                Password = password
            });

            result.Should().NotBeNull();
            result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
            result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Error_invalid_User()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateLoginUseCase();

            var request = new RequestLoginJson
            {
                Email = string.Empty,
                Password = password
            };

            Func<Task> act = async () => { await useCase.Login(request); };

            await act.Should().ThrowAsync<IncorrectLoginException>()
                .Where(e => e.Message.Equals(MessagesException.EMAIL_OR_PASSWORD_INVALID));
        }

        [Fact]
        public async Task Error_invalid_Password()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateLoginUseCase(user);

            var request = new RequestLoginJson
            {
                Email = user.Email,
                Password = "wrong_password"
            };

            Func<Task> act = async () => { await useCase.Login(request); };

            await act.Should().ThrowAsync<IncorrectLoginException>()
                .Where(e => e.Message.Equals(MessagesException.EMAIL_OR_PASSWORD_INVALID));
        }

        private LoginUseCase CreateLoginUseCase(CardManager.Domain.Entities.User? user = null)
        {
            var readRepositoryBuilder = new UserReadRepositoryBuilder();
            var tokenGeneratorBuilder = TokenGeneratorBuilder.Build();
            if (user is not null)
                readRepositoryBuilder.GetEmail(user);

            return new LoginUseCase(readRepositoryBuilder.Build(), tokenGeneratorBuilder);
        }
    }
}
