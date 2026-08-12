using CardManager.Application.UseCase.Transaction.ChangeAmount;
using CardManager.Exceptions;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.Transaction
{
    public class ChangeAmountValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new ChangeAmountValidator();
            var request = RequestChangeAmountJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Amount_Invalid()
        {
            var validator = new ChangeAmountValidator();
            var request = RequestChangeAmountJsonBuilder.Build();
            request.Amount = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CHANGE_AMOUNT);
        }
    }
}
