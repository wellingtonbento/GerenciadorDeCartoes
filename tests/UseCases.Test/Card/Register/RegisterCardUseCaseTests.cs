using CardManager.Application.UseCase.Card.Register;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Mapper;
using CoreTestUtilities.Repositories;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Test.Card.Register
{
    public class RegisterCardUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestCardJsonBuilder.Build();

            var useCase = CreateUseCase();

            var result = await useCase.RegisterCard(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            var request = RequestCardJsonBuilder.Build();
            request.Name = string.Empty;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.RegisterCard(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.NAME_REQUIRED));
        }

        [Fact]
        public async Task Error_CreditLimit_Negative()
        {
            var request = RequestCardJsonBuilder.Build();
            request.CreditLimit = -1;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.RegisterCard(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_CREDIT_LIMIT_INVALID));
        }

        [Fact]
        public async Task Error_CreditBalance_Negative()
        {
            var request = RequestCardJsonBuilder.Build();
            request.CreditBalance = -1;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.RegisterCard(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_CREDIT_BALANCE_INVALID));
        }

        [Fact]
        public async Task Error_Debit_Negative()
        {
            var request = RequestCardJsonBuilder.Build();
            request.Debit = -1;

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.RegisterCard(request);

            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.CARD_DEBIT_BALANCE_INVALID));
        }

        private static RegisterCardUseCase CreateUseCase()
        {
            var user = UserBuilder.Build().user;
            var mapper = MapperBuilder.Build();
            var repository = CardWriteRepositoryBuilder.Build();
            var loggedUser = LoggedUserBuilder.Build(user);
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new RegisterCardUseCase(repository, loggedUser, unitOfWork, mapper);
        }
    }
}
