using CardManager.Application.UseCase.User.ChangePassword;
using CardManager.Exceptions;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.User.ChangePassword
{
    public class ChangePasswordValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_New_Password_Empty()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.NewPassword = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.PASSWORD_REQUIRED);
        }

        [Fact]
        public void Error_New_Password_Too_Short()
        {
            var validator = new ChangePasswordValidator();
            var request = RequestChangePasswordJsonBuilder.Build();
            request.NewPassword = "abc";

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.PASSWORD_EMPTY);
        }
    }
}
