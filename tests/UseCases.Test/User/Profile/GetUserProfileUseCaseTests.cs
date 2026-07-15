using CardManager.Application.UseCase.User.Profile;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Identity;
using CoreTestUtilities.Mapper;
using FluentAssertions;

namespace UseCases.Test.User.Profile
{
    public class GetUserProfileUseCaseTests
    {
        [Fact]
        public async Task Success()
        {
            (var user, var _) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.GetUserProfile();

            result.Should().NotBeNull();
            result.Name.Should().Be(user.Name);
            result.Email.Should().Be(user.Email);
        }


        private static GetUserProfileUseCase CreateUseCase(CardManager.Domain.Entities.User user)
        {
            var loggerUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();

            return new GetUserProfileUseCase(loggerUser, mapper);
        }
    }

}
