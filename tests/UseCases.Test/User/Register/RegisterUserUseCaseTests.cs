using CardManager.Application.UseCase.User.Register;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Mapper;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestUserRegisterJsonBuilder.Build();

            var useCase = CreateUseCase();

            var result = await useCase.ValidateRequest(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
        }

        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            var request = RequestUserRegisterJsonBuilder.Build();

            var useCase = CreateUseCase(request.Email);

            Func<Task> act = async () => await useCase.ValidateRequest(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.EMAIL_ALREADY_REGISTERED));
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            var request = RequestUserRegisterJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.ValidateRequest(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.NAME_EMPTY));
        }

        private static RegisterUserUseCase CreateUseCase(string? email = null)
        {
            var mapper = MapperBuilder.Build();
            var writeRepository = UserWriteRepositoryBuilder.Build();
            var readRepositoryBuilder = new UserReadRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();

            if (string.IsNullOrWhiteSpace(email) == false)
                readRepositoryBuilder.ExistActiveUserWithEmail(email);

            return new RegisterUserUseCase(writeRepository, readRepositoryBuilder.Build(), unitOfWork, mapper);
        }
    }
}
