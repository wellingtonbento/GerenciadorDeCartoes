using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.Transaction.ChangeAmount
{
    public class ChangeAmountValidator : AbstractValidator<RequestChangeAmountJson>
    {
        public ChangeAmountValidator()
        {
            RuleFor(request => request.Amount).GreaterThanOrEqualTo(0).WithMessage(MessagesException.CHANGE_AMOUNT);
        }
    }
}
