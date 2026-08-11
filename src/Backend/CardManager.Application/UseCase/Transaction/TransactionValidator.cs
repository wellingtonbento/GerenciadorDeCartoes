using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.Transaction
{
    public class TransactionValidator : AbstractValidator<RequestRegisterTransaction>
    {
        public TransactionValidator()
        {
            RuleFor(request => request.CardId).GreaterThan(0).WithMessage(MessagesException.CARD_ID_INVALID);
            RuleFor(request => request.PaymentMethod).IsInEnum().WithMessage(MessagesException.PAYMENT_METHOD_INVALID);
            RuleFor(request => request.Amount).GreaterThan(0).WithMessage(MessagesException.TRANSACTION_VALUE_INVALID);
            RuleFor(request => request.Description).NotEmpty().WithMessage(MessagesException.DESCRIPTION_REQUIRED);
        }
    }
}
