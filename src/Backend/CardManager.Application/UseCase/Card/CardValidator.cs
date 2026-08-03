using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.Card
{
    public class CardValidator : AbstractValidator<RequestCardJson>
    {
        public CardValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage(MessagesException.NAME_REQUIRED);
            RuleFor(request => request.Type).IsInEnum().WithMessage(MessagesException.CARD_TYPE_INVALID);
            RuleFor(request => request.CreditLimit).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CARD_CREDIT_LIMIT_INVALID);
            RuleFor(request => request.AmountSpent).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CARD_AMOUNT_SPENT_INVALID);
            RuleFor(request => request.DebitBalance).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CARD_DEBIT_BALANCE_INVALID);
        }
    }
}
