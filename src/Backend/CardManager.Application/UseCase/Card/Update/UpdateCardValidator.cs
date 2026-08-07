using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.Card.Update
{
    public class UpdateCardValidator : AbstractValidator<RequestUpdateCardJson>
    {
        public UpdateCardValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage(MessagesException.NAME_REQUIRED);
            RuleFor(request => request.CreditLimit).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CARD_CREDIT_LIMIT_INVALID);
            RuleFor(request => request.Debit).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CARD_DEBIT_BALANCE_INVALID);
        }
    }
}
