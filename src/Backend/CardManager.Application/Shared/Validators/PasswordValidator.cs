using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.Shared.Validators
{
    public static class PasswordValidator
    {
        internal static IRuleBuilderOptions<TRequest, string> Password<TRequest>(this IRuleBuilderInitial<TRequest, string> ruleBuilder)
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(MessagesException.PASSWORD_REQUIRED)
                .MinimumLength(6)
                .WithMessage(MessagesException.PASSWORD_EMPTY);
        }
    }
}
