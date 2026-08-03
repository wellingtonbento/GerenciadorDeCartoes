using CardManager.Application.UseCase.Card;
using CardManager.Communication.Enums;
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
        public void Error_Type_Invalid()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.Type = (CardType)999;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_TYPE_INVALID);
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
        public void Error_AmountSpent_Negative()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.AmountSpent = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_AMOUNT_SPENT_INVALID);
        }

        [Fact]
        public void Error_DebitBalance_Negative()
        {
            var validator = new CardValidator();
            var request = RequestCardJsonBuilder.Build();
            request.DebitBalance = -1;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_DEBIT_BALANCE_INVALID);
        }
    }
}
