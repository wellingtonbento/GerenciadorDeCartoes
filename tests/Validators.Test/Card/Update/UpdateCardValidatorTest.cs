using CardManager.Application.UseCase.Card.Update;
using CardManager.Exceptions;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.Card.Update
{
    public class UpdateCardValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new UpdateCardValidator();
            var request = RequestUpdateCardJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new UpdateCardValidator();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.NAME_REQUIRED);
        }

        [Fact]
        public void Error_CreditLimit_Negative()
        {
            var validator = new UpdateCardValidator();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.CreditLimit = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_CREDIT_LIMIT_INVALID);
        }

        [Fact]
        public void Error_Debit_Negative()
        {
            var validator = new UpdateCardValidator();
            var request = RequestUpdateCardJsonBuilder.Build();
            request.Debit = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_DEBIT_BALANCE_INVALID);
        }
    }
}
