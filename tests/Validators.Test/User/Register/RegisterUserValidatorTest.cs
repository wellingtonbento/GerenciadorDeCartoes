using CardManager.Application.UseCase.User.Register;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new RegisterUserValidator();

            var request = RequestUserRegisterJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
