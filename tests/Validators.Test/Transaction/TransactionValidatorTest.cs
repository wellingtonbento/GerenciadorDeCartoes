using CardManager.Application.UseCase.Transaction;
using CardManager.Communication.Enums;
using CardManager.Exceptions;
using CoreTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Test.Transaction
{
    public class TransactionValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new TransactionValidator();
            var request = RequestRegisterTransactionBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_CardId_Invalid()
        {
            var validator = new TransactionValidator();
            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = 0;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.CARD_ID_INVALID);
        }

        [Fact]
        public void Error_PaymentMethod_Invalid()
        {
            var validator = new TransactionValidator();
            var request = RequestRegisterTransactionBuilder.Build();
            request.PaymentMethod = (PaymentMethod)99;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.PAYMENT_METHOD_INVALID);
        }

        [Fact]
        public void Error_Amount_Invalid()
        {
            var validator = new TransactionValidator();
            var request = RequestRegisterTransactionBuilder.Build();
            request.Amount = 0;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.TRANSACTION_VALUE_INVALID);
        }

        [Fact]
        public void Error_Description_Empty()
        {
            var validator = new TransactionValidator();
            var request = RequestRegisterTransactionBuilder.Build();
            request.Description = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == MessagesException.DESCRIPTION_REQUIRED);
        }
    }
}
