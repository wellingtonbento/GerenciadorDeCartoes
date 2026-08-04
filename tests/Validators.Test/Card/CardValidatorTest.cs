using CardManager.Application.UseCase.Card;
using CardManager.Exceptions;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.Card
{
    public class CardValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.NAME_REQUIRED);
        }

        [Fact]
        public void Error_CreditLimit_Negative()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.CreditLimit = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_CREDIT_LIMIT_INVALID);
        }

        [Fact]
        public void Error_CreditBalance_Negative()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.CreditBalance = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_CREDIT_BALANCE_INVALID);
        }

        [Fact]
        public void Error_Debit_Negative()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.Debit = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_DEBIT_BALANCE_INVALID);
        }
    }
}
