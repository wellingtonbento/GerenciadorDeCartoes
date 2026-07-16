using CardManager.Application.UseCase.Shared.Validators;
using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.User.Register
{
    public class RegisterUserValidator : AbstractValidator<RequestUserRegisterJson>
    {
        public RegisterUserValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(MessagesException.NAME_EMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(MessagesException.EMAIL_EMPTY);
            RuleFor(user => user.Email).EmailAddress().WithMessage(MessagesException.EMAIL_INVALID);
            RuleFor(user => user.Password).Password();
        }
    }
}
